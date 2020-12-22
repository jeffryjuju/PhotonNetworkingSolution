using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject findMatchButton;
    [SerializeField]
    GameObject searchingPanel;

    // Start is called before the first frame update
    void Start()
    {
        // Connect to Photon Server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"We are connected to Photon on {PhotonNetwork.CloudRegion} server");
        PhotonNetwork.AutomaticallySyncScene = true;
        findMatchButton.SetActive(true);
    }

    public void FindMatch()
    {
        searchingPanel.SetActive(true);
        findMatchButton.SetActive(false);

        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Finding match...");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Cannot find a room. Creating a room.");
        MakeRoom();
    }

    void MakeRoom()
    {
        int randomRoomName = Random.Range(0, 5000);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 2
        };
        PhotonNetwork.CreateRoom("RoomName_" + randomRoomName, roomOptions);
        Debug.Log("Room created, awaiting other players.");
    }

    public void StopSearch()
    {
        searchingPanel.SetActive(false);
        findMatchButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
        Debug.Log("Stopped finding match. Back to Main Menu.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"{PhotonNetwork.CurrentRoom.PlayerCount}/2 players joined. Starting the game");
            // Start the game
            PhotonNetwork.LoadLevel(1);
        }
    }
}
