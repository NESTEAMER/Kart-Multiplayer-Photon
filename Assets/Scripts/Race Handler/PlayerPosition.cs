using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class PlayerPosition : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI positionText;
    public int totalLaps = 3;

    private Dictionary<int, int> playerLaps = new Dictionary<int, int>();

    private bool isFinishLineActive = false;

    public TextMeshProUGUI finishPlacementText;
    private List<int> finishedPlayers = new List<int>();

    public GameObject antiCheatBox; 
    public Dictionary<int, bool> playerFinishLinePass = new Dictionary<int, bool>();


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Collider>().enabled = false;
        Invoke("ActivateFinishLine", 15f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRacePositions();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerLaps[newPlayer.ActorNumber] = 0; 
        UpdateRacePositions();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerLaps.Remove(otherPlayer.ActorNumber); 
        UpdateRacePositions();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isFinishLineActive)
        {
            PhotonView otherPlayerView = other.gameObject.GetComponent<PhotonView>();
            if (otherPlayerView != null)
            {
                int playerId = otherPlayerView.OwnerActorNr;

                if (playerFinishLinePass.ContainsKey(playerId) && playerFinishLinePass[playerId]) 
                {
                    if (!playerLaps.ContainsKey(playerId))
                    {
                        playerLaps[playerId] = 0;
                    }

                    playerLaps[playerId]++;

                    if (playerLaps[playerId] >= totalLaps)
                    {
                        otherPlayerView.RPC("DestroyCar", RpcTarget.AllBuffered);

                        if (!finishedPlayers.Contains(playerId))
                        {
                            finishedPlayers.Add(playerId);
                        }

                        UpdateFinishPlacementText();
                    }

                    playerFinishLinePass[playerId] = false; 

                    UpdateRacePositions();
                }
                else
                {
                    Debug.LogWarning($"Player {playerId} tried to cross the finish line without a pass!");
                }
            }
        }
    }

    void UpdateRacePositions()
    {
        string positionString = "";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            int lapCount = playerLaps.ContainsKey(player.ActorNumber) ? playerLaps[player.ActorNumber] : 0;
            positionString += $"{player.NickName} - Lap: {lapCount}/{totalLaps}\n";
        }

        positionText.text = positionString;
        Debug.Log("Race Positions:\n" + positionString);
    }

    void ActivateFinishLine()
    {
        isFinishLineActive = true;
        GetComponent<Collider>().enabled = true;
    }

    void UpdateFinishPlacementText()
    {
       string placementString = "";

        var finishedPlayersList = PhotonNetwork.PlayerList
            .Where(player => playerLaps.ContainsKey(player.ActorNumber) && playerLaps[player.ActorNumber] >= totalLaps)
            .OrderByDescending(player => playerLaps[player.ActorNumber])
            .ThenBy(player => System.Array.IndexOf(PhotonNetwork.PlayerList, player)); 

        for (int i = 0; i < finishedPlayersList.Count(); i++)
        {
             placementString += $"{i + 1}. {finishedPlayersList.ElementAt(i).NickName}\n";
        }

        finishPlacementText.text = placementString;
        Debug.Log("Finish Placements:\n" + placementString);
    }
}
