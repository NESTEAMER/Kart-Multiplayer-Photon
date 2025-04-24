using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartTrailerGAS : MonoBehaviour
{
    public float speed = 20f; 

    void FixedUpdate()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed; 
    }
}
