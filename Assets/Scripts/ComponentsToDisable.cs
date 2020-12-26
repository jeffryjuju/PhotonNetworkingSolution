using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Script to disable controller of another player.
/// </summary>
public class ComponentsToDisable : MonoBehaviourPun
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                Destroy(componentsToDisable[i]);
            }
        }
    }
}
