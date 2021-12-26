using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTrigger : MonoBehaviour
{
    private PlayerController[] _players;
    private PlayerController _finalPlayer;

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.GetComponent<PlayerController>()) return;
        
        //Deactivate the player that collides with this trigger
        collider.gameObject.SetActive(false);

        //Do a search for all players still present
        _players = FindObjectsOfType<PlayerController>();

        //If only one player remains, make them the winner.
        if (_players != null && _players.Length == 1)
        {
            _finalPlayer = _players[0];

            _finalPlayer.WinTheGame();
        }
    }
}