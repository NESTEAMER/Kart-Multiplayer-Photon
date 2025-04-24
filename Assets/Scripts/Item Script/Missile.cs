using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Missile : MonoBehaviourPunCallbacks
{
    public float launchSpeed = 20f;
    public GameObject explosionPrefab; 
    public float explosionForce = 500f;
    public float explosionRadius = 10f;
    public float destroyTime = 10f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * launchSpeed, ForceMode.Impulse); 
        Destroy(gameObject, destroyTime); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        photonView.RPC("DestroyMissile", RpcTarget.AllBuffered); 
    }

    [PunRPC]
    void DestroyMissile()
    {
        Destroy(gameObject);
    }
}
