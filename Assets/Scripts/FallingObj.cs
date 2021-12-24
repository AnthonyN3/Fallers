using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObj : MonoBehaviour
{
    private System.Random rand;
    public GameObject objectToDrop;


    void Awake()
    {
        rand = new System.Random();
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Drop", 0.5f, 1.5f);
    }

    private void Drop()
    {
        Instantiate(objectToDrop, new Vector3(rand.Next(0, 10),rand.Next(10,20), rand.Next(-24,-18)), Quaternion.identity);
    }

}
