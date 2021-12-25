using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")] 
    [SerializeField] private GameObject _mainScreen;
    [SerializeField] private GameObject _lobbyScreen;

    [Header("Main Screen")] 
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private Button _joinRoomButton;

    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private TMP_InputField _roomNameInputField;
    
    [Header("Lobby Screen")] 
    [SerializeField] private TextMeshProUGUI _playerListText;
    [SerializeField] private Button _startGameButton;

    private void Start()
    {
        _createRoomButton.interactable = false;
        _joinRoomButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        _createRoomButton.interactable = true;
        _joinRoomButton.interactable = true;
    }

    private void SetScreen(GameObject screen)
    {
        _mainScreen.SetActive(false);
        _lobbyScreen.SetActive(false);

        screen.SetActive(true);
    }

    public void OnCreateRoomButton()
    {
        if (_roomNameInputField.text == "") return;
        if (_playerNameInputField.text == "") return;
        NetworkManager.Instance.CreateRoom(_roomNameInputField.text);
    }

    public void OnJoinRoomButton()
    {
        if (_roomNameInputField.text == "") return;
        if (_playerNameInputField.text == "") return;
        NetworkManager.Instance.JoinRoom(_roomNameInputField.text);
    }

    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnJoinedRoom()
    {
        SetScreen(_lobbyScreen);

        UpdateLobbyUI();
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherplayer)
    {
        //no need to RPC here because
        //OnJoinedRoom is only called for the client who just joined
        //OnPlayerLeftRoom gets called for all client in the room
        //So no need for RPC call
        UpdateLobbyUI();
    }

    [PunRPC]
    public void UpdateLobbyUI()
    {
        _playerListText.text = "";

        //display all players currently in the lobby
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            _playerListText.text += player.NickName + "\n";
        }
        
        // only the hose cn start the game
        if (PhotonNetwork.IsMasterClient)
            _startGameButton.interactable = true;
        else
            _startGameButton.interactable = false;
    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(_mainScreen);
    }

    public void OnStartGameButton()
    {
        NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }
}
