using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEditor;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    
    [Header("Stats")]
    public bool GameEnded = false;
    public float TimeToWin;
    
    [SerializeField] private float _invincibleDuration;


    [Header("Player")]
    public PlayerController[] Players;
    public int PlayerWithHat;
    
    [SerializeField] private string _playerPrefabLocation;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private  int _playersInGame;
    
    [Header("Starting Hat")]
    [SerializeField] private GameObject _hatPrefab;
    [SerializeField] private Transform _hatSpawnPosition;
    
    private float _hatPickUpTime;
    private GameObject _playerObj;
    private PlayerController _playerScript;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        photonView.RPC("SpawnStartingHat", RpcTarget.All);
    }


    [PunRPC]
    private void ImInGame()
    {
        _playersInGame++;

        if (_playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    [PunRPC]
    private void SpawnStartingHat()
    {
        PhotonNetwork.Instantiate(_hatPrefab.name, _hatSpawnPosition.position, Quaternion.identity);
    }
    
    private void SpawnPlayer()
    {
        var spawnPointNumber = Random.Range(0, _spawnPoints.Count);
        
        _playerObj = PhotonNetwork.Instantiate(_playerPrefabLocation,
            _spawnPoints[spawnPointNumber].position, Quaternion.identity);
        
        photonView.RPC("RemoveSpawnPoint", RpcTarget.AllBuffered, spawnPointNumber);
        
        _playerScript = _playerObj.GetComponent<PlayerController>();

        //initialize the player
        _playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    private void RemoveSpawnPoint(int spawnIndex)
    {
        _spawnPoints.Remove(_spawnPoints[spawnIndex]);
    }
    public PlayerController GetPlayer(int playerId)
    {
        return Players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObj)
    {
        return Players.First(x => x.gameObject == playerObj);
    }

    [PunRPC]
    public void GiveHat(int playerId, bool initialGive)
    {
        //remove the hat from the currently hatted player
        if (!initialGive)
            GetPlayer(PlayerWithHat).SetHat(false);

        // give the hat to the new player
        PlayerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        GetPlayer(playerId).Audio.PlayHatSound();
        
        _hatPickUpTime = Time.time;
    }

    public bool CanGetHat()
    {
        if (Time.time > _hatPickUpTime + _invincibleDuration) return true;
        else return false;
    }

    [PunRPC]
    private void WinGame(int playerId)
    {
        GameEnded = true;
        PlayerController player = GetPlayer(playerId);

        //set the ui to show who has won
        GameUI.Instance.SetWinText((player.PhotonPlayer.NickName));
        PauseGameAfterWin.Instance.photonView.RPC("PauseGame", RpcTarget.AllBuffered);

        Invoke("GoBackToMenu", 3.0f);
    }

    private void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.Instance.ChangeScene("Menu");
        PauseGameAfterWin.Instance.photonView.RPC("UnpauseGame", RpcTarget.AllBuffered);
    }
}