using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public NetworkingPlayer player;
    public Camera cam;

    public RaycastHit rayHit;
    public LayerMask WhatIsWeapon;

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit, 4.5f, WhatIsWeapon))
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                GameObject weaponGo = rayHit.transform.gameObject;

                if(weaponGo.GetComponent<GunSystem>() != null)
                {
                    GunSystem weapon = weaponGo.GetComponent<GunSystem>();
                    for(var i = 0; i < WeaponManager.Instance.weapons.Length; i++) 
                    {
                        if(weapon.name == WeaponManager.Instance.weapons[i].name)
                        {
                            player.PickupGun(i);
                        }
                    }
                }
            }
        }
    }

    private void dropWeapon()
    {
        if(transform.childCount > 0)
        {
            Transform weaponTr = transform.GetChild(0);
            Rigidbody weaponRb =  weaponTr.GetComponent<Rigidbody>();

            // Turn off shooting script and remove it from Weapon Container
            weaponTr.GetComponent<GunSystem>().enabled = false;
            weaponTr.SetParent(null);

            // Enable phyics on the weapon and add a drop/throw force
            weaponRb.isKinematic = false;
            weaponRb.velocity = (cam.transform.forward + cam.transform.up)*5.0f;
            weaponRb.angularVelocity = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f), Random.Range(-1f,1f) * 10.0f);
        } else {
            Debug.Log("There is noting to drop");
        }
        
    }
}
