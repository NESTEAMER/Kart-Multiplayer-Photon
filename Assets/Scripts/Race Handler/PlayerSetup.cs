using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using System.Linq;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject playerUIPrefab;
    private WheelDrive wheelDriveController;
    public Camera CarCamera;
    private Item item;

    public GameObject[] objectsToDestroy;
    public Camera spectateCamera;

    //item handle stuff
    public int bananaCount = 0;
    public int rocketCount = 0;
    public int smokeCount = 0;

    public GameObject bananaPrefab;
    public GameObject rocketPrefab;
    public GameObject smokePrefab;

    public Sprite bananaSprite;
    public Sprite rocketSprite;
    public Sprite smokeSprite;


    public Transform spawnFront;
    public Transform spawnBack;

    private bool isUsingItem = false;
    private Image itemImage;

    //respawn cooldown
    public float respawnCooldown = 3f;
    private bool isRespawning = false;

    // Start is called before the first frame update
    void Start()
    {
        item = GetComponent<Item>();
        wheelDriveController = GetComponent<WheelDrive>();

        if (photonView.IsMine)
        {
            // Instantiate playerUI
            GameObject playerUIGameObject = Instantiate(playerUIPrefab);
            wheelDriveController.joystick = playerUIGameObject.transform.Find("Fixed Joystick").GetComponent<Joystick>();
            //wheelDriveController.brakeButton = playerUIGameObject.transform.Find("brakeButton").GetComponent<Button>();
            CarCamera.enabled = true;


            playerUIGameObject.transform.Find("itemButton").GetComponent<Button>().onClick.AddListener(() => this.UseItem());
            playerUIGameObject.transform.Find("respawnButton").GetComponent<Button>().onClick.AddListener(() => StartCoroutine(Respawn()));
        }
        else
        {
            wheelDriveController.enabled = false;
            CarCamera.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ItemBox") && photonView.IsMine)
        {
            GiveRandomItem(Random.Range(0, 3));
        }
    }

    private IEnumerator Respawn()
    {
        if (!isRespawning) 
        {
            isRespawning = true; 

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            yield return new WaitForFixedUpdate();

            Transform checkpointsParent = GameObject.Find("Checkpoints").transform;
            Transform[] checkpoints = checkpointsParent.GetComponentsInChildren<Transform>();

            Transform closestCheckpoint = checkpoints.OrderBy(x => Vector3.Distance(x.position, transform.position)).FirstOrDefault();

            photonView.RPC("TeleportToCheckpoint", RpcTarget.AllBuffered, closestCheckpoint.position);

            StartCoroutine(RespawnCooldown()); 
        }
    }

    private IEnumerator RespawnCooldown()
    {
        yield return new WaitForSeconds(respawnCooldown);
        isRespawning = false; 
    }

    public void UseItem()
    {
        if (!isUsingItem && (bananaCount > 0 || rocketCount > 0 || smokeCount > 0))
        {
            isUsingItem = true;
            photonView.RPC("UseItemRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void TeleportToCheckpoint(Vector3 checkpointPosition)
    {
        transform.position = checkpointPosition;
        transform.rotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    [PunRPC]
    void DestroyCar()
    {
        if (photonView.IsMine)
        {
            Destroy(GameObject.Find("PlayerUI(Clone)"));
            GetComponent<WheelDrive>().enabled = false;

            CarCamera.enabled = false;
            spectateCamera.enabled = true;
        }

        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }

    }

    void GiveRandomItem(int itemIndex)
    {
        if (bananaCount == 0 && rocketCount == 0 && smokeCount == 0)
        {
            GameObject itemImage = GameObject.Find("itemImage");

            switch (itemIndex)
            {
                case 0: //banana
                    bananaCount = 1;
                    itemImage.GetComponent<Image>().sprite = bananaSprite;
                    Debug.Log(itemImage.GetComponent<Image>().sprite);
                    break;
                case 1: //rocket
                    rocketCount = 1;
                    itemImage.GetComponent<Image>().sprite = rocketSprite;
                    Debug.Log(itemImage.GetComponent<Image>().sprite);
                    break;
                case 2: //smoke
                    smokeCount = 1;
                    itemImage.GetComponent<Image>().sprite = smokeSprite;
                    Debug.Log(itemImage.GetComponent<Image>().sprite);
                    break;
            }
        }
        else
        {
            Debug.Log("You already have an item!");
        }
    }

    [PunRPC]
    void UseItemRPC()
    {
        GameObject itemImage = GameObject.Find("itemImage");

        if (bananaCount > 0)
        {
            PhotonNetwork.Instantiate(bananaPrefab.name, spawnBack.position, spawnBack.rotation);
            bananaCount = 0;
        }
        else if (rocketCount > 0)
        {
            PhotonNetwork.Instantiate(rocketPrefab.name, spawnFront.position, spawnFront.rotation);
            rocketCount = 0;
        }
        else if (smokeCount > 0)
        {
            PhotonNetwork.Instantiate(smokePrefab.name, spawnBack.position, spawnBack.rotation);
            smokeCount = 0;
        }

        isUsingItem = false;

        if (photonView.IsMine)
        {
            itemImage.GetComponent<Image>().sprite = null;
        }
    }

}
