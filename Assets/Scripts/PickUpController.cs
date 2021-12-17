using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public Camera cam;

    public RaycastHit rayHit;
    public LayerMask WhatIsWeapon;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit, 4.5f, WhatIsWeapon))
            {
                dropWeapon();   // drop weapon if one is currently held
                Transform weaponTr = rayHit.transform;

                weaponTr.GetComponent<Rigidbody>().isKinematic = true;

                weaponTr.SetParent(transform);
                weaponTr.localPosition = Vector3.zero;
                weaponTr.localRotation = Quaternion.Euler(Vector3.zero);
                weaponTr.localScale = Vector3.one;
                
                weaponTr.GetComponent<GunSystem>().enabled = true;
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
            dropWeapon();
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
