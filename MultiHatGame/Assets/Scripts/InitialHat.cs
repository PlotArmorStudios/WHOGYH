using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

//The hat activated at the beginning of the game
//Simply activates the hat on the player that touches it
//The first player to grab it gets the advantage!

public class InitialHat : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.GetComponent<PlayerController>()) return;
     
        var player = collider.GetComponent<PlayerController>();

        GameManager.Instance.GiveHat(player.id, true);
        
        gameObject.SetActive(false);
    }
}
