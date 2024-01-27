using Photon.Pun;
using Photon.Realtime;
using System;
using System.Diagnostics;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Core.MatchMaking
{

    public class GameRoomWithProperties : BaseRoomMatchMaker , IDisposable
    {

        public const string MAP_PROP_KEY = "map";
        public const string GAME_MODE_PROP_KEY = "gm";
        public const string AI_PROP_KEY = "ai";

        GameObject _playerFab;

        public GameRoomWithProperties( GameObject playerFab, LoadBalancingClient loadBalancingClient)
        {
             _playerFab = playerFab;
            LoadBalancingClient = loadBalancingClient;
            LoadBalancingClient.AddCallbackTarget(this);
            
        }


        public void CreateOrJoinRoom(string roomName, byte maxPlayers)
        {
            CreateOrJoin(roomName, maxPlayers);
        }
      
        protected override void CreateOrJoin(string name,byte maxPlayersInRoom = 4)
        {
           
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 12;
            roomOptions.IsOpen = true;
            roomOptions.PublishUserId = true;
            roomOptions.IsVisible = true;
            
            roomOptions.CustomRoomPropertiesForLobby = new string[] { MAP_PROP_KEY, GAME_MODE_PROP_KEY, AI_PROP_KEY };
            roomOptions.CustomRoomProperties = new Hashtable { { MAP_PROP_KEY, 1 }, { GAME_MODE_PROP_KEY, 0 } };
            EnterRoomParams enterRoomParams = new EnterRoomParams();
            enterRoomParams.RoomOptions = roomOptions;
            enterRoomParams.RoomName = name;
          
             LoadBalancingClient.OpJoinOrCreateRoom(enterRoomParams);
            
            
        }


        public void JoinToConcrereRoom(RoomInfo roomInfo)
        {

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.CustomRoomPropertiesForLobby = new string[] { MAP_PROP_KEY, GAME_MODE_PROP_KEY, AI_PROP_KEY };
            roomOptions.CustomRoomProperties = roomInfo.CustomProperties; 
            EnterRoomParams enterRoomParams = new EnterRoomParams();
            enterRoomParams.RoomName = roomInfo.Name;
            enterRoomParams.RoomOptions = roomOptions;
            LoadBalancingClient.OpJoinRoom(enterRoomParams);
        }

        public override void OnCreatedRoom()
        {
            
            base.OnCreatedRoom();
            
        }

        public override void OnJoinedRoom()
        {
           
            base.OnJoinedRoom();
                if(PhotonNetwork.IsMasterClient)
                    PhotonNetwork.LoadLevel(1);

            UnityEngine.Debug.LogWarning("«¿√–”« ¿");
        }
        public override void FindQuickGame()
        {
            LoadBalancingClient.OpJoinRandomRoom();
        }

        public void Dispose()
        {
            LoadBalancingClient.RemoveCallbackTarget(this);
            
        }
    }
}