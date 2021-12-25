using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

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
