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

        collider.gameObject.SetActive(false);

        _players = FindObjectsOfType<PlayerController>();

        if (_players != null && _players.Length == 1)
        {
            _finalPlayer = _players[0];

            _finalPlayer.WinTheGame();
        }
    }
}