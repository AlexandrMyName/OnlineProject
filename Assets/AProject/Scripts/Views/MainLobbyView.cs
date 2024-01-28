using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Core.MatchMaking
{

    public class MainLobbyView : MonoBehaviourPun
    {

        [SerializeField] private Button _joinLobbyButton;
        [SerializeField] private RoomListView _roomListView;
        [SerializeField] private ServerSettings _serverSettings;
        [SerializeField] private List<GameObject> _hidenObjects;
        [SerializeField] private GameObject _playerFab;

        private RoomListCaching _roomList;
         
        private void Awake()
        {
            
            _joinLobbyButton.onClick.AddListener(() => JoinToMainLobby());
        }

        private void Update()
        {
           
            _roomList?.UpdateServices();
        }


        public void JoinToMainLobby()
        {

            _roomList = new RoomListCaching();
            _roomList.ConnectAndJoinLobby(_serverSettings, _playerFab);
            _roomListView.gameObject.SetActive(true);
            _roomListView.Init(_roomList);
        }


        private void OnDestroy()
        {

            _joinLobbyButton.onClick.RemoveAllListeners();
            _roomList?.Dispose();
        }
        
    }
}