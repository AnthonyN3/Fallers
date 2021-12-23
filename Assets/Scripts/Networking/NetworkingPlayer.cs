using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;

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

        var guns = FindObjectsOfType<GunSystem>();
        foreach(var gun in guns) {
            gun.SetPlayer(this);
        }
    }

    private void Update() 
    {
        if(networkObject == null || playerTransform == null) {
            return;
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
