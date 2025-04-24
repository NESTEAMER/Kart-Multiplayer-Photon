using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class OffTrackHandler : MonoBehaviourPunCallbacks
{
    public Transform checkpointsParent;
    private Transform[] checkpoints;

    // Start is called before the first frame update
    void Start()
    {
        checkpoints = checkpointsParent.GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     void OnTriggerEnter(Collider other)
    {
         if (other.CompareTag("Player"))
        {
            PhotonView playerView = other.gameObject.GetComponent<PhotonView>();
            if (playerView != null)
            {
                StartCoroutine(TeleportPlayerToClosestCheckpoint(playerView));
            }
        }
    }

      private System.Collections.IEnumerator TeleportPlayerToClosestCheckpoint(PhotonView playerView)
    {
        yield return new WaitForFixedUpdate(); 

        Transform closestCheckpoint = FindClosestCheckpoint(playerView.transform.position);
        playerView.RPC("Respawn", RpcTarget.AllBuffered, closestCheckpoint.position);
    }

    private Transform FindClosestCheckpoint(Vector3 playerPosition)
    {
        return checkpoints.OrderBy(x => Vector3.Distance(x.position, playerPosition)).FirstOrDefault();
    }

}
