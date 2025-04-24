using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Banana : MonoBehaviourPunCallbacks
{
    public float explosionForce = 1000f;
    public float explosionRadius = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            photonView.RPC("DestroyBanana", RpcTarget.AllBuffered); 
        }
    }

    [PunRPC]
    void DestroyBanana()
    {
        Destroy(gameObject);
    }
}
