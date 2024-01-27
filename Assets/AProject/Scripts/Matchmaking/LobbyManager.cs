using Core.MatchMaking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LobbyManager : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject _playerFab;
    [SerializeField] private Transform _spawnNet;
   

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        
        if (_playerFab == null)
        {  

            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
                PhotonNetwork.Instantiate(_playerFab.name, _spawnNet.position, Quaternion.identity, 0);

                Debug.LogWarning(PhotonNetwork.CurrentRoom.Name);
                Debug.LogWarning(PhotonNetwork.CurrentRoom.PlayerCount + " кол-во игроков" ) ;
                Debug.Log(BaseRoomMatchMaker.LoadBalancingClient.State);
              
                Debug.LogWarning(PhotonNetwork.CloudRegion);
                   
 
        }
    }


    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
     
    public bool LeaveRoom() => PhotonNetwork.LeaveRoom();
    

    public void QuitApplication()
    {
        Application.Quit();
    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
         
        Debug.LogWarning("Успешно вошел в комнату :" + newPlayer.NickName);
        Debug.LogWarning("IsMasterClient :" + newPlayer.IsMasterClient);
        Debug.LogWarning("ID:  :" + newPlayer.UserId);
        Debug.LogWarning("IsInactive  :" + newPlayer.IsInactive);
        
    }



}
