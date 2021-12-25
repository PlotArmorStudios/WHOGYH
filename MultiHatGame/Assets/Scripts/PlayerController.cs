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
    public SoundHandler Audio { get; private set; }

    [PunRPC]
    public void Initialize(Player player)
    {
        PhotonPlayer = player;
        id = player.ActorNumber;

        GameManager.Instance.Players[id - 1] = this;

        //give the first player the hat
        //if (id == 1)
          //  GameManager.Instance.GiveHat(id, true);
        
        if (!photonView.IsMine)
            _rigidbody.isKinematic = true;
    }

    private void Start()
    {
        Audio = GetComponent<SoundHandler>();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (CurrentHatTime >= GameManager.Instance.TimeToWin && !GameManager.Instance.GameEnded)
            {
                WinTheGame();
            }
        }
        
        if (!photonView.IsMine)
            return;

        if (PauseGameAfterWin.Active) return;
        
        Move();
        
        if (Input.GetKeyDown((KeyCode.Space)))
        {
            TryJump();
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

    private void TryJump()
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) stream.SendNext(CurrentHatTime);
        else if (stream.IsReading) CurrentHatTime = (float) stream.ReceiveNext();
    }
}