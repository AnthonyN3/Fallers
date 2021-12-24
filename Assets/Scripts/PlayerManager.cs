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
    private TextMeshProUGUI flagText;

    private void Awake() 
    {
        curHealth = maxHealth;
        healthText = GameObject.Find("health_text").GetComponent<TextMeshProUGUI>();
        flagText = GameObject.Find("flag_icon").GetComponent<TextMeshProUGUI>();
        flagText.enabled = false;
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
            
            if(carryingFlag) 
            {
                carryingFlag = false;
                player.DroppedFlag();
                flagText.enabled = false;
            }

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
           AudioSource.PlayClipAtPoint(SoundManager.instance.sounds[9], transform.position);
           flagText.enabled = true;
        }
        if(other.CompareTag("blueflag") && player.networkObject.team == 'R') 
        {
            player.PickedFlag();
            AudioSource.PlayClipAtPoint(SoundManager.instance.sounds[9], transform.position);
            flagText.enabled = true;
        }

        if(other.CompareTag("redflag") && player.networkObject.team == 'R') 
        {
            Debug.Log("Collided with friendly flag");
            float flagDistanceFromSpawn = Vector3.Distance(other.gameObject.transform.position, GameObject.Find("Red_Flag_Spawn").transform.position);
            if(flagDistanceFromSpawn > 3.0f)
            {
                Debug.Log("Flag far from spawn, respawning");
                player.RespawnedFlag();
                AudioSource.PlayClipAtPoint(SoundManager.instance.sounds[9], transform.position);
            } 
            else 
            {
                Debug.Log("Flag close to spawn, ignoring");
                if(carryingFlag)
                {
                    Debug.Log("Flag close to spawn, scoring");
                    player.RespawnedFlag();
                    player.Scored();
                    AudioSource.PlayClipAtPoint(SoundManager.instance.sounds[9], transform.position);
                    flagText.enabled = false;
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
                AudioSource.PlayClipAtPoint(SoundManager.instance.sounds[9], transform.position);
            } 
            else 
            {
                Debug.Log("Flag close to spawn, ignoring");
                if(carryingFlag)
                {
                    Debug.Log("Flag close to spawn, scoring");
                    player.RespawnedFlag();
                    player.Scored();
                    AudioSource.PlayClipAtPoint(SoundManager.instance.sounds[9], transform.position);
                    flagText.enabled = false;
                }
            }
        }

        if(other.CompareTag("hp"))
        {
            int healthInPack = other.GetComponent<Hp>().PickUp();;
            int healthNeeded = maxHealth - curHealth;

            curHealth += healthNeeded > healthInPack ? healthInPack : healthNeeded;
            AudioSource.PlayClipAtPoint(SoundManager.instance.sounds[8], transform.position);
        }
        if (other.CompareTag("ammo"))
        {
            // ammo stuff
            GunSystem weapon = player.currentGun;
            int ammoInPack = other.GetComponent<AmmoPack>().PickUp();
            int ammoNeeded = weapon.reservedAmmoMax - weapon.reservedAmmoCur;

            weapon.reservedAmmoCur += ammoNeeded > ammoInPack ?  ammoInPack : ammoNeeded;
            AudioSource.PlayClipAtPoint(SoundManager.instance.sounds[8], transform.position);
        }
    }
}
