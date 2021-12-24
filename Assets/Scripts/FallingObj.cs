using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObj : MonoBehaviour
{
    private System.Random rand;
    public GameObject objectToDrop;
    public GameObject redRagdoll;
    public GameObject blueRagdoll;


    void Awake()
    {
        rand = new System.Random();
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("DropBox", 0.25f, 1.5f);
        InvokeRepeating("DropRed", 0.75f, 2f);
        InvokeRepeating("DropBlue", 1.0f, 2f);
    }

    private void DropBox()
    {
        Instantiate(objectToDrop, new Vector3(rand.Next(0, 10),rand.Next(10,20), rand.Next(-24,-18)), Random.rotation);
    }

    private void DropRed()
    {
        Instantiate(redRagdoll, new Vector3(rand.Next(0, 10),rand.Next(10,20), rand.Next(-24,-18)), Random.rotation);
    }

    private void DropBlue()
    {
        Instantiate(blueRagdoll, new Vector3(rand.Next(0, 10),rand.Next(10,20), rand.Next(-24,-18)), Random.rotation);
    }


}
