using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


namespace Core.MatchMaking
{

    public class MainLobbyView : MonoBehaviour
    {

        [SerializeField] private Button _joinLobbyButton;
        [SerializeField] private RoomListView _roomListView;
        [SerializeField] private ServerSettings _serverSettings;
        
        private RoomListCaching _roomList;
        


        private void Awake() => _joinLobbyButton.onClick.AddListener(() => OnJoinToMainLobby());


        private void Update()
        {
            _roomList?.UpdateServices();
        }


        public void OnJoinToMainLobby()
        {

            _roomList = new RoomListCaching();
            _roomList.JoinLobby(_serverSettings);
            _roomListView.gameObject.SetActive(true);
            _roomListView.Init(_roomList);
        }


        private void OnDestroy() =>  _joinLobbyButton.onClick.RemoveAllListeners();
        
    }
}