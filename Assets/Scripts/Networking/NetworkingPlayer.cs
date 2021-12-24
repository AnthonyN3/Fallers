using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using TMPro;

public class NetworkingPlayer : NetworkedPlayerBehavior
{
    public Transform orientationTransform;

    public Transform playerTransform;
    public GunSystem currentGun;
    public Transform gunContainer;
    public Transform clientViewGunContainer;

    public GameObject redflag;
    public GameObject blueflag;

    private Vector3 spawnPos;

    public GameObject model;
    public Animator animator;

    public GameObject endScreen;

    private IEnumerator Start()
    {
        while (networkObject == null) yield return null;
        
        if(networkObject.IsOwner) {
            endScreen = GameObject.Find("EndScreen");   
            endScreen.SetActive(false);
        } 
    }

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

        model.SetActive(false);

        networkObject.name = GetInstanceID();

        playerTransform.GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(PickTeam());
    }

    private void Update() 
    {
        if(networkObject == null || playerTransform == null) {
            return;
        }    

        if(gameObject.name != networkObject.name.ToString()) {
            gameObject.name = networkObject.name.ToString();
        }

        animator.SetFloat("walking", Mathf.Abs(networkObject.walking));

        if(!networkObject.IsOwner) {
            playerTransform.position = networkObject.pos;
            orientationTransform.rotation = networkObject.rot;
            return;
        }

        networkObject.pos = playerTransform.position;
        networkObject.rot = orientationTransform.rotation;
        networkObject.walking = Input.GetAxis("Vertical") + Input.GetAxis("Horizontal");
    }

    public void DoSound(int soundIndex, Vector3 pos)
    {
        if(networkObject == null) {
            return;
        }

        networkObject.SendRpc(RPC_PLAY_SOUND, Receivers.All, soundIndex, pos);
    }

    public override void PlaySound(RpcArgs args)
    {
        int soundIndex = args.GetNext<int>();
        Vector3 soundPos = args.GetNext<Vector3>();

        AudioSource.PlayClipAtPoint(SoundManager.instance.sounds[soundIndex], soundPos);
    }

    public void DoJump()
    {
        if(!networkObject.IsOwner) {
            return;
        }

        networkObject.SendRpc(RPC_JUMP, Receivers.All);
    }

    public override void Jump(RpcArgs args)
    {
        animator.SetTrigger("jump");
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

        spawnPos = transform.position;
    }

    public override void SetTeam(RpcArgs args)
    {
        uint playerId = args.GetNext<uint>();
        char team = args.GetNext<char>();

        Debug.Log("Adding player " + playerId + " to team " + team);
        
        LobbyManager.Instance.AddNewPlayer(team, this);
        model.GetComponent<SkinnedMeshRenderer>().material.color = team == 'R' ? Color.red : Color.blue;
    }

    public void PickupGun(int gunid)
    {
        networkObject.SendRpc(RPC_SET_GUN, Receivers.AllBuffered, gunid);
    }

    public override void SetGun(RpcArgs args)
    {
        int gunIndex = args.GetNext<int>();

        if(currentGun != null) {
            Destroy(currentGun.GetClientViewWeapon());
            Destroy(currentGun.gameObject);
        }

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

    public void ApplyDamage(string targetName, float damage)
    {
        networkObject.SendRpc(RPC_DO_DAMAGE, Receivers.AllBuffered, targetName, damage);
    }

    public override void DoDamage(RpcArgs args)
    {
        string targetName = args.GetNext<string>();
        float damage = args.GetNext<float>();

        Debug.Log("Player " + networkObject.Owner.NetworkId + " damaged " + targetName + " for " + damage);

        var target = GameObject.Find(targetName);
        if(target != null) {
            target.GetComponentInChildren<PlayerManager>().ApplyDamage(damage);
        }
    }

    public void Die() 
    {
        if(networkObject == null) {
            Debug.Log("Network object is null");
            return;
        }

        if(networkObject.IsOwner) {
            Debug.Log("Player " + networkObject.Owner.NetworkId + " died");
            if(networkObject.team == 'R') {
                playerTransform.position = LobbyManager.Instance.RedSpawn.position;
            } else {
                playerTransform.position = LobbyManager.Instance.BlueSpawn.position;
            }
        }
    }

    public void PickedFlag()
    {
        char team = networkObject.team;
        string playerName = gameObject.name;

        networkObject.SendRpc(RPC_PICKUP_FLAG, Receivers.AllBuffered, team, playerName);
    }

    public void DroppedFlag()
    {
        char team = networkObject.team;
        string playerName = gameObject.name;
        Vector3 newPos = playerTransform.position;

        networkObject.SendRpc(RPC_DROP_FLAG, Receivers.AllBuffered, team, playerName, newPos);
    }

    public void RespawnedFlag()
    {
        char team = networkObject.team;

        networkObject.SendRpc(RPC_RESPAWN_FLAG, Receivers.AllBuffered, team);
    }

    public void Scored()
    {
        char team = networkObject.team;
        string playerName = gameObject.name;

        GameObject.Find(playerName).GetComponentInChildren<PlayerManager>().DroppedFlag();

        networkObject.SendRpc(RPC_TEAM_SCORE, Receivers.AllBuffered, team, playerName);
    }

    public override void PickupFlag(RpcArgs args)
    {
        char team = args.GetNext<char>();
        string playerName = args.GetNext<string>();

        Debug.Log("Player " + playerName + " picked up the flag for team " + team);
        if(team == 'R')
        {
            LobbyManager.Instance.BlueFlag.SetActive(false);
            GameObject.Find(playerName).GetComponent<NetworkingPlayer>().blueflag.SetActive(true);
        } 
        else 
        {
            LobbyManager.Instance.RedFlag.SetActive(false);
            GameObject.Find(playerName).GetComponent<NetworkingPlayer>().redflag.SetActive(true);
        }

        GameObject.Find(playerName).GetComponentInChildren<PlayerManager>().CarryingFlag();
    }

    public override void DropFlag(RpcArgs args)
    {
        char team = args.GetNext<char>();
        string playerName = args.GetNext<string>();
        Vector3 newPos = args.GetNext<Vector3>();

        Debug.Log("Player " + playerName + " respawned the flag for team " + team);
        if(team == 'R')
        {
            LobbyManager.Instance.BlueFlag.SetActive(true);
            LobbyManager.Instance.BlueFlag.transform.position = newPos;
            GameObject.Find(playerName).GetComponent<NetworkingPlayer>().blueflag.SetActive(false);
        }
        else 
        {
            LobbyManager.Instance.RedFlag.SetActive(true);
            LobbyManager.Instance.RedFlag.transform.position = newPos;
            GameObject.Find(playerName).GetComponent<NetworkingPlayer>().redflag.SetActive(false);
        }

        GameObject.Find(playerName).GetComponentInChildren<PlayerManager>().DroppedFlag();
    }

    public override void RespawnFlag(RpcArgs args)
    {
        char team = args.GetNext<char>();

        Debug.Log("Flag respawned for team " + team);
        if(team == 'R')
        {
            LobbyManager.Instance.RedFlag.SetActive(true);
            LobbyManager.Instance.RedFlag.transform.position = GameObject.Find("Red_Flag_Spawn").transform.position;
        }
        else 
        {
            LobbyManager.Instance.BlueFlag.SetActive(true);
            LobbyManager.Instance.BlueFlag.transform.position = GameObject.Find("Blue_Flag_Spawn").transform.position;
        }
    }

    public override void TeamScore(RpcArgs args)
    {
        char team = args.GetNext<char>();
        string playerName = args.GetNext<string>();

        Debug.Log("Team " + team + " scored");
        if(team == 'R')
        {
            LobbyManager.Instance.BlueFlag.SetActive(true);
            LobbyManager.Instance.BlueFlag.transform.position = GameObject.Find("Blue_Flag_Spawn").transform.position;
            GameObject.Find(playerName).GetComponent<NetworkingPlayer>().blueflag.SetActive(false);
            LobbyManager.Instance.RedScored();
        }
        else
        {
            LobbyManager.Instance.RedFlag.SetActive(true);
            LobbyManager.Instance.RedFlag.transform.position = GameObject.Find("Red_Flag_Spawn").transform.position;
            GameObject.Find(playerName).GetComponent<NetworkingPlayer>().redflag.SetActive(false);
            LobbyManager.Instance.BlueScored();
        }

        GameObject.Find(playerName).GetComponentInChildren<PlayerManager>().DroppedFlag();

        if(networkObject.IsServer) {
            if(LobbyManager.Instance.redScore == 10 || LobbyManager.Instance.blueScore == 10)
            {
                networkObject.SendRpc(RPC_END_GAME, Receivers.AllBuffered, LobbyManager.Instance.redScore > LobbyManager.Instance.blueScore ? 'R' : 'B');
            }
        }
    }

    public override void EndGame(RpcArgs args)
    {
        char winner = args.GetNext<char>();
        endScreen.SetActive(true);

        endScreen.transform.Find("Winner").GetComponent<TextMeshProUGUI>().text = winner == 'R' ? "Red Team" : "Blue Team";
    }
}
