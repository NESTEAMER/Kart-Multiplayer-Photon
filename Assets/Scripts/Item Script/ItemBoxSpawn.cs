using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxSpawn : MonoBehaviour
{
    public GameObject itemBoxPrefab;
    public float respawnTime = 5f;
    private bool isRespawning = false;

    // Start is called before the first frame update
    void Start()
    {
        SpawnItemBox();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount == 0 && !isRespawning)
        {
            isRespawning = true; 
            StartCoroutine(RespawnItemBox());
        }
    }

    IEnumerator RespawnItemBox()
    {
        yield return new WaitForSeconds(respawnTime);

        SpawnItemBox();
        isRespawning = false;
    }

    void SpawnItemBox()
    {
        Instantiate(itemBoxPrefab, transform.position, transform.rotation, transform); 
    }
}
