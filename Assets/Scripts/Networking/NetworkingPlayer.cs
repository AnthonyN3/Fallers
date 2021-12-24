using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;

public class NetworkingPlayer : NetworkedPlayerBehavior
{
    public Transform orientationTransform;

    public Transform playerTransform;
    public GunSystem currentGun;
    public Transform gunContainer;
    public Transform clientViewGunContainer;

    protected override void NetworkStart()
    {
        base.NetworkStart();

        if(networkObject == null) {
            return;
        }

        if(!networkObject.IsOwner) {
            transform.Find("CameraContainer").gameObject.SetActive(false);
            playerTransform.GetComponent<PlayerMovement>().enabled = false;
            Destroy(playerTransform.GetComponent<Rigidbody>());
            return;
        }

        networkObject.name = GetInstanceID();

        playerTransform.GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(PickTeam());
    }

    IEnumerator PickTeam()
    {
        yield return new WaitForSeconds(2.0f);
        Debug.Log("Count on Red Team: " + LobbyManager.Instance.RedTeam.Count + " Count on Blue Team: " + LobbyManager.Instance.BlueTeam.Count);

        if(LobbyManager.Instance.RedTeam.Count < LobbyManager.Instance.BlueTeam.Count) {
            networkObject.team = 'R';
        } else {
            networkObject.team = 'B';
        }

        Debug.Log("Setting your team to " + networkObject.team);

        networkObject.SendRpc(RPC_SET_TEAM, Receivers.AllBuffered,  networkObject.Owner.NetworkId, networkObject.team);
        networkObject.SendRpc(RPC_SET_GUN, Receivers.AllBuffered, 0);

        playerTransform.GetComponent<Rigidbody>().isKinematic = false;
    }

    public override void SetTeam(RpcArgs args)
    {
        uint playerId = args.GetNext<uint>();
        char team = args.GetNext<char>();

        Debug.Log("Adding player " + playerId + " to team " + team);
        
        LobbyManager.Instance.AddNewPlayer(team, this);
        playerTransform.GetComponent<Renderer>().material.color = team == 'R' ? Color.red : Color.blue;
    }

    public override void SetGun(RpcArgs args)
    {
        int gunIndex = args.GetNext<int>();

        var weaponGO = WeaponManager.Instance.weapons[gunIndex];
        var weaponInstance = Instantiate(weaponGO);
        var clientWeaponInstance = Instantiate(weaponGO);
        Destroy(clientWeaponInstance.GetComponent<Rigidbody>());
        
        weaponInstance.transform.SetParent(gunContainer);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;

        clientWeaponInstance.transform.SetParent(clientViewGunContainer);
        clientWeaponInstance.transform.localPosition = Vector3.zero;
        clientWeaponInstance.transform.localRotation = Quaternion.identity;
        clientWeaponInstance.GetComponent<GunSystem>().enabled = false;
        
        currentGun = weaponInstance.GetComponent<GunSystem>();
        currentGun.SetPlayer(this);
        currentGun.SetClientViewWeapon(clientWeaponInstance);

        if(networkObject.IsOwner)
        {
            currentGun.SetCamera(Camera.main);
            clientWeaponInstance.SetActive(false);
        }
    }

    public void DoShoot(Vector3 hitPoint, Vector3 hitNormal)
    {
        networkObject.SendRpc(RPC_SHOOT_GUN, Receivers.Others, hitPoint, hitNormal);
    }

    public override void ShootGun(RpcArgs args)
    {
        Vector3 hitPoint = args.GetNext<Vector3>();
        Vector3 hitNormal = args.GetNext<Vector3>();

        currentGun.spawnBulletHole(hitPoint, hitNormal);
        currentGun.muzzleFlash.Play();
        currentGun.muzzleFlash2.Play();
    }

    private void Update() 
    {
        if(networkObject == null || playerTransform == null) {
            return;
        }    

        if(gameObject.name != networkObject.name.ToString()) {
            gameObject.name = networkObject.name.ToString();
        }

        if(!networkObject.IsOwner) {
            playerTransform.position = networkObject.pos;
            orientationTransform.rotation = networkObject.rot;
            return;
        }

        networkObject.pos = playerTransform.position;
        networkObject.rot = orientationTransform.rotation;
    }
}
