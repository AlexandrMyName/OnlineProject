using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core.MatchMaking
{

    public class RoomListCaching : ILobbyCallbacks, IConnectionCallbacks, IDisposable
    {

        private TypedLobby _customLobby = new TypedLobby("customLobby", LobbyType.Default);
        private LoadBalancingClient _loadBalancingClient;
        public Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string,RoomInfo>();
        private GameRoomWithProperties _gameRoomWithProperties;
        GameObject _playerFab;

        public void ConnectAndJoinLobby(ServerSettings serverSettings, GameObject playerFab)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            _playerFab = playerFab;
             
            _loadBalancingClient = PhotonNetwork.NetworkingClient;

            _loadBalancingClient.AddCallbackTarget(this);
            serverSettings.DevRegion = "ru";
            serverSettings.AppSettings.FixedRegion = "ru";
              _loadBalancingClient.ConnectUsingSettings(serverSettings.AppSettings);
             
        }


        public void UpdateServices()
        {
            _loadBalancingClient?.Service();
        }


        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {

            Debug.Log($"UpdateCachedRoomList ROOM !!!  {roomList.Count}");
            for (int i = 0; i < roomList.Count; i++)
            {
                RoomInfo info = roomList[i];
                Debug.Log(info.Name + " ROOM !!!");
                if (info.RemovedFromList)
                {
                    _cachedRoomList.Remove(info.Name);
                }
                else
                {
                    _cachedRoomList[info.Name] = info;
                }
            }
        }

  
        public void OnConnected()
        {
            Debug.Log($"<color=green>[OnConnected]</color> ");
        }


        public void OnConnectedToMaster()
        {

           
            Debug.Log($"<color=green>[OnConnectedToMaster]</color> ");

          
           _loadBalancingClient.OpJoinLobby(_customLobby);
            
        }

        


        public void OnCustomAuthenticationFailed(string debugMessage)
        {
            Debug.Log($"<color=red>[OnCustomAuthenticationFailed]</color> ");
        }


        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            Debug.Log($"<color=yellow>[OnCustomAuthenticationResponse]</color> ");
        }


        public void OnDisconnected(DisconnectCause cause)
        {
            _cachedRoomList.Clear();
        }


        public void OnJoinedLobby()
        {
            Debug.Log($"<color=green>[ ���� � ������� �����]</color> ");
             
            _gameRoomWithProperties = new(_playerFab,_loadBalancingClient);
            _gameRoomWithProperties.CreateOrJoinRoom("RoomLobby", 4);

        }


        public void OnLeftLobby()
        {

           // _cachedRoomList.Clear();
        }


        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            Debug.Log($"<color=green>[OnLobbyStatisticsUpdate]</color> ");
        }


        public void OnRegionListReceived(RegionHandler regionHandler)
        {
            Debug.Log($"<color=green>[OnRegionListReceived]</color> ");
        }


        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log($"<color=green>[OnRoomListUpdate] Count: {roomList.Count}</color> ");
           // UpdateCachedRoomList(roomList);
        }

        public void Dispose()
        {
            _loadBalancingClient.RemoveCallbackTarget(this);
            _gameRoomWithProperties?.Dispose();
        }
    }
}