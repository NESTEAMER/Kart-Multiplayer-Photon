using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KartRaceManager : MonoBehaviour
{
   [SerializeField]
    GameObject playerPrefab;
    GameObject SpawnLocations;


    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (playerPrefab != null)
            {
                int randomPoint = Random.Range(-10, 10);
                PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPoint, 0f,
                randomPoint),
                Quaternion.identity);
            }
            else
            {
                Debug.Log("Player prefab error!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
