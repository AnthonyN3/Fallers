using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
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
    //public GameObject muzzleFlash, bulletHoles;

    // UI & Text
    public TextMeshProUGUI ammoText;

    private void Awake() 
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;  
    }

    private void Update()
    {
        MyInput();
        ammoText.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void MyInput()
    {
        shooting = allowButtonHold ? (Input.GetKey(KeyCode.Mouse0)) : (Input.GetKeyDown(KeyCode.Mouse0));

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // More spread if player is running/moving
        // if(rb.velocity.magnitude > 0)
        //     spread = spread*2f;

        Vector3 direction = cam.transform.forward + new Vector3(x, y, 0);

        if (Physics.Raycast(cam.transform.position, direction, out rayHit, range, WhatIsEnemy))
        {
            //Debug.Log(rayHit.collider.name);
            // if(rayHit.collider.CompareTag("Player"))
        }

        bulletsLeft--;
        bulletsShot--;

        // Camera Shake
        StartCoroutine(camShake.Shake(camShakeDuration, camShakeMagnitude));

        Invoke("ResetShot", timeBetweenShooting);

        // Bullet Hole and Muzzle Flash
        // Instantiate(bulletHoles, rayHit.point, Quaternion.Euler(0,180,0));
        // Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        if (bulletsShot > 0 && bulletsLeft > 0) // this would only be useful for the burst weapons
            Invoke("Shoot", timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
