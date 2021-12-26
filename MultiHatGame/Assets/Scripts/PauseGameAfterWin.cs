using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

//Simply stops player's input
//PunRPC attribute allows the Active
//boolean to be modified for all players when the method is called

public class PauseGameAfterWin : MonoBehaviourPunCallbacks
{
    public static PauseGameAfterWin Instance;
    public static bool Active { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [PunRPC]
    public void PauseGame()
    {
        Active = true;
    }

    [PunRPC]
    public void UnpauseGame()
    {
        Active = false;
    }
}
