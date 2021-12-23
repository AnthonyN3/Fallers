using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    public int ammoAmount;
    public float respawnTime;

    private void Update() 
    {
        transform.Rotate(new Vector3(0, 150, 0) * Time.deltaTime, Space.World);  
    }

    public int PickUp()
    {   
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        Invoke("Respawn", respawnTime);
        return ammoAmount;
    }

    private void Respawn()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
        gameObject.GetComponent<Collider>().enabled = true;
    }
}
