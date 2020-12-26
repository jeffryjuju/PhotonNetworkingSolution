using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    // Room info
    public static PhotonRoom room;
    private PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;

    // Player info
    Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    public int playersInGame;

    // Delayed start
    private bool readyToCount;
    private bool readyToStart;
    public float startingTime;
    private float lessThanMaxPlayer;
    private float atMaxPlayer;
    private float timeToStart;
    private string difficultyLevel;

    public GameObject mainLobbyGameObject;
    public GameObject roomGameObject;
    public Transform playersPanel;
    public GameObject playerListingPrefab;
    public GameObject startButton;
    public GameObject difficultyOptions;
    public GameObject difficultyGetter;

    private void Awake()
    {
        // Create singleton
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        // subscribe to functions
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayer = startingTime;
        atMaxPlayer = 6;
        timeToStart = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            if (playersInRoom == 1)
            {
                RestartTimer();
            }
            if (!isGameLoaded)
            {
                if (readyToStart)
                {
                    atMaxPlayer -= Time.deltaTime;
                    lessThanMaxPlayer = atMaxPlayer;
                    timeToStart = atMaxPlayer;
                }
                else if (readyToCount)
                {
                    lessThanMaxPlayer -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayer;
                }
                Debug.Log($"Counting down, {timeToStart}");
                if (timeToStart <= 0)
                {
                    StartGame();
                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("New player joined the room.");

        mainLobbyGameObject.SetActive(false);
        roomGameObject.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
            difficultyOptions.SetActive(true);
        }
        else
        {
            difficultyGetter.SetActive(true);
        }
        ClearPlayerListings();
        ListPlayers();

        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            Debug.Log($"({playersInRoom}/{MultiplayerSettings.multiplayerSettings.maxPlayers}) person(s) in this room.");
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        /*else
        {
            StartGame();
        }*/
    }

    /// <summary>
    /// Deletes the list of players inside the room.
    /// </summary>
    void ClearPlayerListings()
    {
        for (int i = playersPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(playersPanel.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Shows all players inside the room.
    /// </summary>
    void ListPlayers()
    {
        if (PhotonNetwork.InRoom)
        {
            foreach(Player player in PhotonNetwork.PlayerList)
            {
                GameObject tempListing = Instantiate(playerListingPrefab, playersPanel);
                TMP_Text tempText = tempListing.transform.GetChild(0).GetComponent<TMP_Text>();
                tempText.text = player.NickName;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Updates player data when a new player joins
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has joined the room.");

        ClearPlayerListings();
        ListPlayers();

        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        // Delay start
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            Debug.Log($"({playersInRoom}/{MultiplayerSettings.multiplayerSettings.maxPlayers}) person(s) in this room.");
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void StartGame()
    {
        isGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSettings.multiplayerScene);
    }

    void RestartTimer()
    {
        lessThanMaxPlayer = startingTime;
        timeToStart = startingTime;
        atMaxPlayer = 6;
        readyToCount = false;
        readyToStart = false;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if (currentScene == MultiplayerSettings.multiplayerSettings.multiplayerScene)
        {
            isGameLoaded = true;
            if (MultiplayerSettings.multiplayerSettings.delayStart)
            {
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            else
            {
                RPC_CreatePlayer();
            }
        }
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), transform.position, Quaternion.identity);
    }

    //private void CreatePlayer()
    //{
    //    PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), transform.position, Quaternion.identity);
    //}

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"{otherPlayer.NickName} has left the game.");
        playersInRoom--;
        ClearPlayerListings();
        ListPlayers();
    }

    /// <summary>
    /// Leaves the room.
    /// </summary>
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            roomGameObject.SetActive(false);
            mainLobbyGameObject.SetActive(true);
        }
    }
}
