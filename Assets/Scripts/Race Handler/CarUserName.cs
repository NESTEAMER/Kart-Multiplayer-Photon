using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class CarUserName : MonoBehaviour
{
    [SerializeField] PhotonView photonView;
    [Space]
    [SerializeField] TMP_Text usernameText;
    [SerializeField] GameObject usernameCanvas;

    void Awake()
    {
        SetUsername(); 
    }

    void SetUsername()
    {
        if (!photonView.IsMine)
        {
            usernameCanvas.SetActive(true);
            usernameText.text = photonView.Owner.NickName;
        }
    }
}
