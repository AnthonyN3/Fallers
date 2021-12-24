using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public NetworkingPlayer player;

    public int maxHealth;
    public int curHealth;

    private TextMeshProUGUI healthText;

    private void Awake() 
    {
        curHealth = maxHealth;
        healthText = GameObject.Find("health_text").GetComponent<TextMeshProUGUI>();
    }

    private void Update() 
    {
        healthText.SetText(curHealth.ToString());
    }

    public void ApplyDamage(float dmg) 
    {
        curHealth -= (int)dmg;
        if(curHealth <= 0) 
        {
            curHealth = 0;
            player.Die();
            curHealth = maxHealth;
        }
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
            GunSystem weapon = player.currentGun;
            int ammoInPack = other.GetComponent<AmmoPack>().PickUp();
            int ammoNeeded = weapon.reservedAmmoMax - weapon.reservedAmmoCur;

            weapon.reservedAmmoCur += ammoNeeded > ammoInPack ?  ammoInPack : ammoNeeded;
        }
    }
}
