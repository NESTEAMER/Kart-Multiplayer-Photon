using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Smoke : MonoBehaviourPunCallbacks
{
    public float destroyTime = 10f;

    void Start()
    {
        photonView.RPC("DestroySmoke", RpcTarget.AllBuffered, destroyTime); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void DestroySmoke(float time)
    {
        Destroy(gameObject, time);
    }
}
