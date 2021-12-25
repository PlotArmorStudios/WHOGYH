using Photon.Pun;
using UnityEngine;

public class SoundHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private AudioSource _audioSource;


    public void PlayHatSound()
    {
        photonView.RPC("PlayHatSoundRPC", RpcTarget.All);
    }
    [PunRPC]
    private void PlayHatSoundRPC()
    {
        _audioSource.Play();
    }
}