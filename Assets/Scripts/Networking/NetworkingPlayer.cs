using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;

public class NetworkingPlayer : NetworkedPlayerBehavior
{
    public Transform playerTransform;

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

        var guns = FindObjectsOfType<GunSystem>();
        foreach(var gun in guns) {
            gun.SetPlayer(this);
        }
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
            playerTransform.rotation = networkObject.rot;
            return;
        }

        networkObject.pos = playerTransform.position;
        networkObject.rot = playerTransform.rotation;
    }
}
