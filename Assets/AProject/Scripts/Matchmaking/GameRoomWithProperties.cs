using Photon.Pun;
using Photon.Realtime;
using System.Diagnostics;
using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Core.MatchMaking
{

    public class GameRoomWithProperties : BaseRoomMatchMaker 
    {

        public const string MAP_PROP_KEY = "map";
        public const string GAME_MODE_PROP_KEY = "gm";
        public const string AI_PROP_KEY = "ai";

        

        public GameRoomWithProperties(LoadBalancingClient loadBalancingClient)
        {
             
            LoadBalancingClient = loadBalancingClient;
            LoadBalancingClient.AddCallbackTarget(this);
            PhotonNetwork.NetworkingClient = LoadBalancingClient;
        }


        public void CreateNewRoom(string roomName, byte maxPlayers)
        {
            CreateRoom(roomName, maxPlayers);
        }
      
        protected override void CreateRoom(string name,byte maxPlayersInRoom = 4)
        {
           
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayersInRoom;
            roomOptions.IsOpen = true;
            roomOptions.PublishUserId = true;
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
            UnityEngine.Debug.Log($"{LoadBalancingClient.IsConnected} IsConnected | {LoadBalancingClient.CurrentRoom.Name} CurrentRoom Name" );
             

            PhotonNetwork.LoadLevel(1); 
        }
        public override void FindQuickGame()
        {
            LoadBalancingClient.OpJoinRandomRoom();
        }

    }
}