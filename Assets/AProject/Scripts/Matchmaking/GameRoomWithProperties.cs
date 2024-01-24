using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Core.MatchMaking
{

    public class GameRoomWithProperties : BaseRoomMatchMaker 
    {

        public const string MAP_PROP_KEY = "map";
        public const string GAME_MODE_PROP_KEY = "gm";
        public const string AI_PROP_KEY = "ai";

        private LoadBalancingClient loadBalancingClient;


        public GameRoomWithProperties(LoadBalancingClient loadBalancingClient)
        {

            loadBalancingClient.AddCallbackTarget(this);

            this.loadBalancingClient = loadBalancingClient;
            base.LoadBalancingClient = loadBalancingClient;
        }


        public void CreateNewRoom(string roomName, byte maxPlayers)
        {
            CreateRoom(roomName, maxPlayers);
        }
      
        protected override void CreateRoom(string name,byte maxPlayersInRoom = 4)
        {
           
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.CustomRoomPropertiesForLobby = new string[] { MAP_PROP_KEY, GAME_MODE_PROP_KEY, AI_PROP_KEY };
            roomOptions.CustomRoomProperties = new Hashtable { { MAP_PROP_KEY, 1 }, { GAME_MODE_PROP_KEY, 0 } };
            EnterRoomParams enterRoomParams = new EnterRoomParams();
            enterRoomParams.RoomOptions = roomOptions;
            enterRoomParams.RoomName = name;
            loadBalancingClient.OpCreateRoom(enterRoomParams);
        }


        public void JoinToConcrereRoom(RoomInfo roomInfo)
        {

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.CustomRoomPropertiesForLobby = new string[] { MAP_PROP_KEY, GAME_MODE_PROP_KEY, AI_PROP_KEY };
            roomOptions.CustomRoomProperties = roomInfo.CustomProperties; 
            EnterRoomParams enterRoomParams = new EnterRoomParams();
            enterRoomParams.RoomName = roomInfo.Name;
            enterRoomParams.RoomOptions = roomOptions;
            loadBalancingClient.OpJoinRoom(enterRoomParams);
        }


        public override void FindQuickGame()
        {
            LoadBalancingClient.OpJoinRandomRoom();
        }

    }
}