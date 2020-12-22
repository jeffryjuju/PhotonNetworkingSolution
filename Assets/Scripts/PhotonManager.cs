using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] SpawnPoints;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        int player = 0;
        if (!PhotonNetwork.IsMasterClient)
        {
            player = 1;
        }
        GameObject Player = PhotonNetwork.Instantiate("Player", SpawnPoints[player].transform.position, Quaternion.identity);
    }
}
