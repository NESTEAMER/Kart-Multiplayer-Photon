using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AntiCheatBox : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerView = other.gameObject.GetComponent<PhotonView>();
            if (playerView != null)
            {
                int playerId = playerView.OwnerActorNr;

                GameObject finishLineObject = GameObject.Find("gate-finish"); 
                if (finishLineObject != null)
                {
                    PlayerPosition playerPositionScript = finishLineObject.GetComponent<PlayerPosition>();
                    if (playerPositionScript != null)
                    {
                        if (playerPositionScript.playerFinishLinePass.ContainsKey(playerId))
                        {
                            playerPositionScript.playerFinishLinePass[playerId] = true;
                        }
                        else
                        {
                            playerPositionScript.playerFinishLinePass.Add(playerId, true);
                        }
                    }
                    else
                    {
                        Debug.LogError("PlayerPosition script not found");
                    }
                }
                else
                {
                    Debug.LogError("Finish line object not found");
                }
            }
        }
    }
}
