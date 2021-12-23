using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    public TextMeshProUGUI healthText;
    public GameObject weaponContainer;

    private void Awake() 
    {
        curHealth = maxHealth;
    }

    private void Update() 
    {
        healthText.SetText(curHealth.ToString());
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("hp"))
        {
            if(curHealth != maxHealth)
                curHealth += other.GetComponent<Hp>().PickUp();
        }
        if (other.CompareTag("ammo"))
        {
            // ammo stuff
            GunSystem weapon = weaponContainer.transform.GetChild(0).GetComponent<GunSystem>();
            int ammoInPack = other.GetComponent<AmmoPack>().PickUp();
            int ammoNeeded = weapon.reservedAmmoMax - weapon.reservedAmmoCur;

            weapon.reservedAmmoCur += ammoNeeded > ammoInPack ?  ammoInPack : ammoNeeded;
        }
    }
}
