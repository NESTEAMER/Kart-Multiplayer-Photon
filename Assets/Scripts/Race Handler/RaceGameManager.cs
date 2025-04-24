using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RaceGameManager : MonoBehaviour
{
    [SerializeField]
    GameObject playerPrefab;

    [SerializeField]
    Transform spawnPointsParent;
    // Start is called before the first frame update
    void Start()
    {
        if (playerPrefab != null && spawnPointsParent != null)
            {
                //spawnpoint getter
                int spawnPointCount = spawnPointsParent.childCount; 

                //get player
                int playerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber; 

                //anti overlap meassure
                int spawnIndex = (playerActorNumber - 1) % spawnPointCount; 
                Transform spawnPoint = spawnPointsParent.GetChild(spawnIndex);


                PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                Debug.Log("Player prefab error!");
            }
        }

    // Update is called once per frame
    void Update()
    {
        
    }
}

