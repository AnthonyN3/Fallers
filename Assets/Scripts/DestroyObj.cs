using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObj : MonoBehaviour
{
    void Start()
    {
        Invoke("Rip", 10);
    }

    private void Rip()
    {
        Destroy(gameObject);
    }
}
