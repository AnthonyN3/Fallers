using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public NetworkingPlayer player;

    public int maxHealth;
    public int curHealth;

    private bool carryingFlag = false;

    private TextMeshProUGUI healthText;

    private void Awake() 
    {
        curHealth = maxHealth;
        healthText = GameObject.Find("health_text").GetComponent<TextMeshProUGUI>();
    }

    private void Update() 
    {
        if(!player.networkObject.IsOwner) return;
        if(transform.position.y < -10)
        { 
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            player.Die();
        }
        healthText.SetText(curHealth.ToString());
    }

    public void ApplyDamage(float dmg) 
    {
        curHealth -= (int)dmg;
        if(curHealth <= 0) 
        {
            curHealth = 0;
            player.DroppedFlag();
            player.Die();
            curHealth = maxHealth;
        }
    }

    public void CarryingFlag()
    {
        carryingFlag = true;
    }

    public void DroppedFlag()
    {
        carryingFlag = false;
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if(!player.networkObject.IsOwner) return;

        if(other.CompareTag("redflag") && player.networkObject.team == 'B') 
        {
           player.PickedFlag();
        }
        if(other.CompareTag("blueflag") && player.networkObject.team == 'R') 
        {
            player.PickedFlag();
        }

        if(other.CompareTag("redflag") && player.networkObject.team == 'R') 
        {
            Debug.Log("Collided with friendly flag");
            float flagDistanceFromSpawn = Vector3.Distance(other.gameObject.transform.position, GameObject.Find("Red_Flag_Spawn").transform.position);
            if(flagDistanceFromSpawn > 3.0f)
            {
                Debug.Log("Flag far from spawn, respawning");
                player.RespawnedFlag();
            } 
            else 
            {
                Debug.Log("Flag close to spawn, ignoring");
                if(carryingFlag)
                {
                    Debug.Log("Flag close to spawn, scoring");
                    player.RespawnedFlag();
                    player.Scored();
                }
            }
        }

        if(other.CompareTag("blueflag") && player.networkObject.team == 'B') 
        {
            Debug.Log("Collided with friendly flag");
            float flagDistanceFromSpawn = Vector3.Distance(other.gameObject.transform.position, GameObject.Find("Blue_Flag_Spawn").transform.position);
            if(flagDistanceFromSpawn > 3.0f)
            {
                Debug.Log("Flag far from spawn, respawning");
                player.RespawnedFlag();
            } 
            else 
            {
                Debug.Log("Flag close to spawn, ignoring");
                if(carryingFlag)
                {
                    Debug.Log("Flag close to spawn, scoring");
                    player.RespawnedFlag();
                    player.Scored();
                }
            }
        }

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
