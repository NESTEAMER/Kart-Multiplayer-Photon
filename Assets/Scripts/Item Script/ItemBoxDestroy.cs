using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemBoxDestroy : MonoBehaviourPunCallbacks
{

    void OnTriggerEnter(Collider other)
    {
            Destroy(gameObject); 
    }

}
