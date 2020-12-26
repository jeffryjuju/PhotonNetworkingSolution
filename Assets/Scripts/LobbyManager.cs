using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hastable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Script for matchmaking and creating lobby.
/// </summary>
public class LobbyManager : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public static LobbyManager lobby;

    [Header("Create Room")]
    public string playerName;
    public string roomName;
    public int roomSize;
    public string roomDifficulty;

    [Header("Joining Other Room")]
    public string specificRoomName;

    [Header("Panels")]
    public GameObject MainPanel;
    // public GameObject roomListingPrefab;
    // public Transform roomsPanel;

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
        roomDifficulty = "easy";
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
    /// Joining a specific (private) match.
    /// </summary>
    public void JoinSpecificMatch()
    {
        SetPlayerName();
        PhotonNetwork.JoinRoom(specificRoomName);
        Debug.Log($"Joining {specificRoomName} room. Please wait...");
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
        Debug.Log("Unable to join.");
    }

    /// <summary>
    /// Gets player's name input data.
    /// </summary>
    /// <param name="nameInput">Player's intended name.</param>
    public void OnPlayerNameChanged(string nameInput)
    {
        playerName = nameInput;
    }

    /// <summary>
    /// Gets room name input data to create a room.
    /// </summary>
    /// <param name="roomNameInput">Room's size</param>
    public void OnRoomNameChanged(string roomNameInput)
    {
        roomName = roomNameInput;
    }

    /// <summary>
    /// Gets room size input data.
    /// </summary>
    /// <param name="sizeInput"></param>
    public void OnRoomSizeChanged(string sizeInput)
    {
        roomSize = int.Parse(sizeInput);
    }

    /// <summary>
    /// Gets specific room name input to join.
    /// </summary>
    /// <param name="roomNameInput"></param>
    public void OnSpecificRoomNameChanged(string roomNameInput)
    {
        specificRoomName = roomNameInput;
    }

    /// <summary>
    /// Sets difficulty level.
    /// </summary>
    /// <param name="difficultyOption"></param>
    public void OnSetDifficultyChanged(int difficultyOption)
    {
        if (difficultyOption == 0)
        {
            roomDifficulty = "easy";
        }
        if (difficultyOption == 1)
        {
            roomDifficulty = "medium";
        }
        if (difficultyOption == 2)
        {
            roomDifficulty = "hard";
        }
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

        // Custom Room Properties
        Hastable RoomCustomProperties = new Hastable();
        RoomCustomProperties.Add("difficulty", roomDifficulty);
        roomOptions.CustomRoomProperties = RoomCustomProperties;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
        Debug.Log($"Creating room {roomName} for {roomSize} person(s), awaiting other players.");
    }

}
