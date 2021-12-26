using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.PlayerLoop;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private GameObject _hatObject;

    [HideInInspector]
    public float CurrentHatTime;

    [Header("Components")]
    [SerializeField] private Rigidbody _rigidbody;
    public Player PhotonPlayer;
    public PlayerSoundHandler Audio { get; private set; }

    [PunRPC]
    public void Initialize(Player player)
    {
        PhotonPlayer = player;
        
        //Give the player an int id based on the network's id of the client
        id = player.ActorNumber;

        GameManager.Instance.Players[id - 1] = this;
        
        //Separate all of the clients' inputs from each other
        if (!photonView.IsMine)
            _rigidbody.isKinematic = true;
    }

    private void Start()
    {
        Audio = GetComponent<PlayerSoundHandler>();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (CurrentHatTime >= GameManager.Instance.TimeToWin && !GameManager.Instance.GameEnded)
            {
                //Call "Win the game" only through one client and RPC it to all other clients
                WinTheGame();
            }
        }
        
        if (!photonView.IsMine)
            return;

        if (PauseGameAfterWin.Active) return;
        
        Move();
        
        if (Input.GetKeyDown((KeyCode.Space)))
        {
            Jump();
        }
        
        //track the amount of time we are wearing the hat
        if (_hatObject.activeInHierarchy)
            CurrentHatTime += Time.deltaTime;
    }

    public void WinTheGame()
    {
        GameManager.Instance.GameEnded = true;
        GameManager.Instance.photonView.RPC("WinGame", RpcTarget.All, id);
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal") * _moveSpeed;
        float z = Input.GetAxis("Vertical") * _moveSpeed;

        _rigidbody.velocity = new Vector3(x, _rigidbody.velocity.y, z);
    }

    private void Jump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 0.7f))
        {
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    public void SetHat(bool hasHat)
    {
        _hatObject.SetActive(hasHat);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine) return;

        // did we hit another player
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance.GetPlayer(collision.gameObject).id == GameManager.Instance.PlayerWithHat)
            {
                if (GameManager.Instance.CanGetHat())
                {
                    GameManager.Instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
    }

    //Write and read the value of CurrentHatTime to all players
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //client writing value to server
        if(stream.IsWriting) stream.SendNext(CurrentHatTime);
        //client reading value from server
        else if (stream.IsReading) CurrentHatTime = (float) stream.ReceiveNext();
    }
}