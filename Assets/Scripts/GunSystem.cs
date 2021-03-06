using System.Collections;
using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    private NetworkingPlayer player;

    private GameObject clientViewWeapon;

    //public bool enableWeapon;
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, reservedAmmoMax, reservedAmmoCur,bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    bool shooting, readyToShoot, reloading;

    // References
    public Camera cam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask WhatIsEnemy;

    // Camera Shake Info
    public CameraShake camShake;
    public float camShakeMagnitude, camShakeDuration;

    // MuzzleFlash and BulletHoles
    public GameObject bulletHoles;
    public ParticleSystem muzzleFlash;
    public ParticleSystem muzzleFlash2;

    // UI & Text
    public TextMeshProUGUI ammoText;
    private bool setup = false;

    public void SetPlayer(NetworkingPlayer player) {
        this.player = player;

        ammoText = GameObject.Find("ammo_text").GetComponent<TextMeshProUGUI>();

        bulletsLeft = magazineSize;
        readyToShoot = true;
        reservedAmmoCur = reservedAmmoMax;

        if (transform.parent != null && transform.parent.name.Equals("WeaponContainer"))
        { 
            //enableWeapon = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<GunSystem>().enabled = true;
        }
        else
        { 
            //enableWeapon = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<GunSystem>().enabled = false;
        }

        setup = true;
    }

    public void SetCamera(Camera cam) {
        this.cam = cam;
        camShake = cam.gameObject.GetComponent<CameraShake>();
    }

    public void SetClientViewWeapon(GameObject clientViewWeapon) {
        this.clientViewWeapon = clientViewWeapon;
        this.muzzleFlash2 = clientViewWeapon.transform.Find("MuzzleFlashEffect").GetComponent<ParticleSystem>();
    }

    public GameObject GetClientViewWeapon() {
        return clientViewWeapon;
    }

    private void Update()
    {
        if(!setup) {
            return;
        }

        if(!player.networkObject.IsOwner)
        {
            return;
        }

        //if (enableWeapon)
        MyInput();
        ammoText.SetText(bulletsLeft + " / " + reservedAmmoCur);
    }

    private void MyInput()
    {
        if(!player.networkObject.IsOwner)
        {
            return;
        }

        shooting = allowButtonHold ? (Input.GetKey(KeyCode.Mouse0)) : (Input.GetKeyDown(KeyCode.Mouse0));

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading && reservedAmmoCur > 0)
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        } 
        
        if (Input.GetMouseButtonDown(0) && bulletsLeft <= 0)
        {
            player.DoSound(2, transform.position);
        }

        // if(Input.GetKeyDown(KeyCode.P))
        //     LobbyManager.Instance.PrintList();
    }

    private void Shoot()
    {
        if(!player.networkObject.IsOwner)
        {
            return;
        }

        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        float z = Random.Range(-spread, spread);

        // More spread if player is running/moving
        // if(rb.velocity.magnitude > 0)
        //     spread = spread*2f;

        Vector3 direction = cam.transform.forward + new Vector3(x, y, z);

        if (Physics.Raycast(cam.transform.position, direction, out rayHit, range))
        {
            //Debug.Log(rayHit.collider.name);
            if(rayHit.collider.CompareTag("Player"))
            {
                NetworkingPlayer networkedPlayer = rayHit.collider.transform.parent.GetComponent<NetworkingPlayer>();
                if(networkedPlayer != null)
                {
                    var hitTeam = LobbyManager.Instance.GetTeam(networkedPlayer.networkObject.NetworkId);
                    if(hitTeam != LobbyManager.Instance.GetTeam(player.networkObject.NetworkId))
                    {
                        player.ApplyDamage(networkedPlayer.gameObject.name, damage);
                        AudioSource.PlayClipAtPoint(SoundManager.instance.sounds[3], transform.position);
                    }
                }
            }
        }

        bulletsLeft--;
        bulletsShot--;

        // Camera Shake
        StartCoroutine(camShake.Shake(camShakeDuration, camShakeMagnitude));

        Invoke("ResetShot", timeBetweenShooting);

        // Bullet Hole and Muzzle Flash
        //GameObject particleObject = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        player.DoSound(0, muzzleFlash.transform.position);
        muzzleFlash.Play();
        spawnBulletHole(rayHit.point, rayHit.normal);
        player.DoSound(1, rayHit.point);

        //Destroy(particleObject, 0.25f);

        if (bulletsShot > 0 && bulletsLeft > 0) // this would only be useful for the burst/shotgun weapons
            Invoke("Shoot", timeBetweenShots);

        player.DoShoot(rayHit.point, rayHit.normal);
    }

    public void spawnBulletHole(Vector3 pos, Vector3 normal)
    {
        Instantiate(bulletHoles, pos, Quaternion.LookRotation(normal));
    }

    private void ResetShot()
    {
        if(!player.networkObject.IsOwner)
        {
            return;
        }

        readyToShoot = true;
    }

    private void Reload()
    {
        if(!player.networkObject.IsOwner)
        {
            return;
        }

        reloading = true;
        StartCoroutine(ReloadAnimation(reloadTime));
    }

    private IEnumerator ReloadAnimation(float duration)
    {
        if(!player.networkObject.IsOwner)
        {
            yield break;
        }

        Vector3 startRotation = transform.localEulerAngles;
        float endRotation = startRotation.x - 360.0f;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float xRotation = Mathf.Lerp(startRotation.x, endRotation, elapsed / duration) % 360.0f;
            transform.localEulerAngles = new Vector3(xRotation, startRotation.y, startRotation.z);

            yield return null;
        }

        int bulletsReloaded = magazineSize - bulletsLeft;
        if(reservedAmmoCur >= bulletsReloaded)
        {
            bulletsLeft += bulletsReloaded;
            reservedAmmoCur -= bulletsReloaded;
        } else {
            bulletsLeft += reservedAmmoCur;
            reservedAmmoCur = 0;
        }
        reloading = false;
    }

    public void resetSystem()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
}
