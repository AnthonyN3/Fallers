using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;

public class NetworkingPlayer : NetworkedPlayerBehavior
{
    protected override void NetworkStart()
    {
        base.NetworkStart();

        if(networkObject == null) {
            return;
        }

        if(!networkObject.IsOwner) {
            transform.parent.Find("CameraContainer").gameObject.SetActive(false);
            GetComponent<PlayerMovement>().enabled = false;
            GameObject.DestroyImmediate(GetComponent<Rigidbody>());
            return;
        }

        var guns = FindObjectsOfType<GunSystem>();
        foreach(var gun in guns) {
            gun.SetPlayer(this);
        }
    }

    private void Update() 
    {
        if(networkObject == null) {
            return;
        }    

        if(!networkObject.IsOwner) {
            transform.position = networkObject.pos;
            transform.rotation = networkObject.rot;
            return;
        }

        networkObject.pos = transform.position;
        networkObject.rot = transform.rotation;
    }
}
