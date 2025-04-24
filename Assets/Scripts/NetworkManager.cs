using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI Panel")]
    public InputField playerNameInput;
    public GameObject loginUIPanel;
    public GameObject creditsUIPanel;

    [Header("Game Options UI Panel")]
    public GameObject gameOptionsUIPanel;

    [Header("Create Room UI Panel")]
    public GameObject createRoomUIPanel;
    public InputField roomNameInputField;
    public InputField maxPlayerInputField;
    public TMP_Dropdown maxPlayerDropDownField;


    [Header("Inside Room UI Panel")]
    public GameObject insiderRoomUIPanel;
    public Text roomInfoText;
    public GameObject playerListPrefab;
    public GameObject playerListContent;
    public GameObject startGameButton;
    public TMP_Dropdown selectedLevel;

    [Header("Room List UI Panel")]
    public GameObject roomListUIPanel;
    public GameObject roomListEnryPrefab;
    public GameObject roomListParentGameObject;

    [Header("Join Random Room UI Panel")]
    public GameObject joinRandomRoomUIPanel;

    [Header("Connection Status")]
    public Text connectionStatusText;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListGameObject;
    private Dictionary<int, GameObject> playerListGameObject;

    #region Unity methods

    void ClearRoomListView()
    {
        foreach(var obj in roomListGameObject.Values)
        {
            Destroy(obj);
        }
        roomListGameObject.Clear();
    }

    void OnJoinRoomButtonClicked(string roomName)
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        ActivatePanel(loginUIPanel.name);
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObject = new Dictionary<string, GameObject>();
        playerListGameObject = new Dictionary<int, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        connectionStatusText.text = "Connection status: "
            + PhotonNetwork.NetworkClientState;
    }

    public void ActivatePanel(string panelNameToBeActivated)
    {
        loginUIPanel.SetActive(
            panelNameToBeActivated.Equals(loginUIPanel.name));

        gameOptionsUIPanel.SetActive(
            panelNameToBeActivated.Equals(gameOptionsUIPanel.name));

        createRoomUIPanel.SetActive(
            panelNameToBeActivated.Equals(createRoomUIPanel.name));

        insiderRoomUIPanel.SetActive(
            panelNameToBeActivated.Equals(insiderRoomUIPanel.name));

        roomListUIPanel.SetActive(
            panelNameToBeActivated.Equals(roomListUIPanel.name));

        joinRandomRoomUIPanel.SetActive(
            panelNameToBeActivated.Equals(joinRandomRoomUIPanel.name));
    }

    #endregion

    #region UI Callbacks

    public void OnCancelButtonClicked()
    {
        ActivatePanel(gameOptionsUIPanel.name);
    }
    
    public void OnStartGameButtonClicked()
    {
        Debug.Log(selectedLevel.ToString());
        if(PhotonNetwork.IsMasterClient)
        {
            string sceneName = selectedLevel.options[selectedLevel.value].text;
            PhotonNetwork.LoadLevel(sceneName);
        }
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        ActivatePanel(joinRandomRoomUIPanel.name);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnShowRoomListButtonClicked()
    {
        if(!PhotonNetwork.InLobby) 
        {
            PhotonNetwork.JoinLobby();
        }
        ActivatePanel(roomListUIPanel.name);
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 
            (byte) int.Parse(maxPlayerDropDownField.options[maxPlayerDropDownField.value].text);
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;
        if(!string.IsNullOrEmpty(playerName))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        } else
        {
            Debug.Log("Player name is invalid");
        }
    }

    public void OnCreditsButtonClicked()
    {
        creditsUIPanel.SetActive(!creditsUIPanel.activeSelf); 
    }

    #endregion

    #region Photon Callbacks

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        var temp = Instantiate(playerListPrefab);
        temp.transform.SetParent(playerListContent.transform);
        temp.transform.localScale = Vector3.one;
        temp.transform.Find("PlayerNameText")
               .GetComponent<Text>().text = newPlayer.NickName;

        roomInfoText.text = "Room name: "
            + PhotonNetwork.CurrentRoom.Name + " "
            + "Player/Max. Players: "
            + PhotonNetwork.CurrentRoom.PlayerCount + "/"
            + PhotonNetwork.CurrentRoom.MaxPlayers;

        if (newPlayer.ActorNumber == 
            PhotonNetwork.LocalPlayer.ActorNumber)
        {
            temp.transform.Find("PlayerIndicator")
                .gameObject.SetActive(true);
        }
        else
        {
            temp.transform.Find("PlayerIndicator")
                .gameObject.SetActive(false);
        }
        playerListGameObject.Add(newPlayer.ActorNumber, temp);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomInfoText.text = "Room name: "
            + PhotonNetwork.CurrentRoom.Name + " "
            + "Player/Max. Players: "
            + PhotonNetwork.CurrentRoom.PlayerCount + "/"
            + PhotonNetwork.CurrentRoom.MaxPlayers;

        Destroy(playerListGameObject[otherPlayer.ActorNumber].gameObject);
        playerListGameObject.Remove(otherPlayer.ActorNumber);

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.SetActive(true);
            selectedLevel.interactable = true;
        }
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(gameOptionsUIPanel.name);
        foreach(GameObject obj in playerListGameObject.Values)
        {
            Destroy(obj);
        }

        playerListGameObject.Clear();         
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        foreach(RoomInfo room in roomList)
        {
            if(!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if(cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList.Remove(room.Name);
                }
            } else
            {
                if(cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList[room.Name] = room;
                } else
                {
                    cachedRoomList.Add(room.Name, room);
                }
            }

            
        }
        
        foreach(RoomInfo room in cachedRoomList.Values)
        {
            var temp = Instantiate(roomListEnryPrefab);
            temp.transform
                .SetParent(roomListParentGameObject.transform);
            temp.transform.localScale = Vector3.one;
            temp.transform.Find("RoomNameText")
                .GetComponent<Text>().text = room.Name;
            temp.transform.Find("RoomPlayersText")
               .GetComponent<Text>().text = room.PlayerCount + "/" 
               + room.MaxPlayers;

            temp.transform.Find("JoinRoomButton")
                .GetComponent<Button>().onClick.AddListener(
                () => OnJoinRoomButtonClicked(room.Name));

            roomListGameObject.Add(room.Name, temp);
        }
    }

    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName 
            + " is connected to Photon");
        ActivatePanel(gameOptionsUIPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName 
            + " joined to " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(insiderRoomUIPanel.name);

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.SetActive(true);
            selectedLevel.interactable = true;
        } else
        {
            startGameButton.SetActive(false);
            selectedLevel.interactable = false;
        }

        roomInfoText.text = "Room name: "
            + PhotonNetwork.CurrentRoom.Name + " "
            + "Player/Max. Players: " 
            + PhotonNetwork.CurrentRoom.PlayerCount + "/"
            + PhotonNetwork.CurrentRoom.MaxPlayers;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            var temp = Instantiate(playerListPrefab);
            temp.transform.SetParent(playerListContent.transform);
            temp.transform.localScale = Vector3.one;
            temp.transform.Find("PlayerNameText")
                .GetComponent<Text>().text = player.NickName;

            if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                temp.transform.Find("PlayerIndicator")
                    .gameObject.SetActive(true);
            }
            else
            {
                temp.transform.Find("PlayerIndicator")
                    .gameObject.SetActive(false);
            }
            playerListGameObject.Add(player.ActorNumber, temp);
        }
    }
    #endregion

}
