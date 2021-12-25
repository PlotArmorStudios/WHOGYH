using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] PlayerContainers;
    public TextMeshProUGUI WinText;
    public static GameUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializePlayerUI();
    }

    private void InitializePlayerUI()
    {
        // loop through all containers
        for (int x = 0; x < PlayerContainers.Length; x++)
        {
            PlayerUIContainer container = PlayerContainers[x];

            //only enable and modify the UI Coninaers we need 
            if (x < PhotonNetwork.PlayerList.Length)
            {
                container.Obj.SetActive(true);
                container.NameText.text = PhotonNetwork.PlayerList[x].NickName;
                container.HatTimeSlider.maxValue = GameManager.Instance.TimeToWin;
            }
            else
                container.Obj.SetActive(false);
        }
    }

    private void Update()
    {
        UpdatePlayerUI();
    }

    private void UpdatePlayerUI()
    {
        for (int x = 0; x < GameManager.Instance.Players.Length; x++)
        {
            if (GameManager.Instance.Players[x] != null)
            {
                PlayerContainers[x].HatTimeSlider.value = GameManager.Instance.Players[x].CurrentHatTime;
            }
        }
    }

    public void SetWinText(string winnerName)
    {
        WinText.gameObject.SetActive(true);
        WinText.text = winnerName + " wins!";
    }
}

[Serializable]
public class PlayerUIContainer
{
    public GameObject Obj;
    public TextMeshProUGUI NameText;
    public Slider HatTimeSlider;
}