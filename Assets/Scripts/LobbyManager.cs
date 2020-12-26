using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

/// <summary>
/// Script for matchmaking and creating lobby.
/// </summary>
public class LobbyManager : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public static LobbyManager lobby;

    public string playerName;
    public string roomName;
    public int roomSize;
    public GameObject MainPanel;
    // public GameObject roomListingPrefab;
    // public Transform roomsPanel;
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI roomSizeText;

    private void Awake()
    {
        lobby = this; // Creates the singleton
    }

    // Start is called before the first frame update
    void Start()
    {
        // Connect to Photon Server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"You are connected to Photon on {PhotonNetwork.CloudRegion} server");
        PhotonNetwork.AutomaticallySyncScene = true;
        
        MainPanel.SetActive(true);
    }

    /// <summary>
    /// Sets player name.
    /// </summary>
    void SetPlayerName()
    {
        if (playerName == null)
            PhotonNetwork.NickName = "Player "+ Random.Range(0, 100);
        else
            PhotonNetwork.NickName = playerName;
    }

    /// <summary>
    /// Random matchmaking logic.
    /// </summary>
    public void JoinRandomMatch()
    {
        SetPlayerName();
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Finding match...");
    }

    /// <summary>
    /// Executed whenever player unable to find random room.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Cannot find a room. No room available.");
        // MakeRoom();
    }

    public void OnPlayerNameChanged(string nameInput)
    {
        playerName = nameInput;
    }

    public void OnRoomNameChanged(string roomNameInput)
    {
        roomName = roomNameInput;
    }

    public void OnRoomSizeChanged(string sizeInput)
    {
        roomSize = int.Parse(sizeInput);
    }

    /// <summary>
    /// Creating a new room for matchmaking.
    /// </summary>
    public void CreateRoom()
    {
        SetPlayerName();
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)roomSize
        };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        Debug.Log($"Creating room {roomName} for {roomSize} person(s), awaiting other players.");
        roomNameText.text = $"Room Name: {roomName}";
        roomSizeText.text = $"Max Player: {roomSize}";
    }

}
