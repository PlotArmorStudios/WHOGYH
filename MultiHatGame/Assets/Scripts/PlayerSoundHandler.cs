using Photon.Pun;
using UnityEngine;

/*Behavior for storing player's sound data.
It inherits from MonoBehaviourPunCallBacks instead
of MonoBehaviour in order to have access to the "photonView" variable,
which is just an initialized variable in the parent class
that references the PhotonView conponent.
*/

public class PlayerSoundHandler : MonoBehaviourPunCallbacks 
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