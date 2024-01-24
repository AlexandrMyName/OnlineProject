using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core.MatchMaking
{

    public class QuickGame : BaseRoomMatchMaker
    {

        private LoadBalancingClient _loadBalancingClient;
        

        public override void FindQuickGame() => _loadBalancingClient.OpJoinRandomOrCreateRoom(null,null);
         

        protected override void CreateRoom(string name, byte maxPlayersInRoom  = 4)
        {

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayersInRoom;
            EnterRoomParams enterRoomParams = new EnterRoomParams();
            enterRoomParams.RoomOptions = roomOptions;
            enterRoomParams.RoomName = name;
            _loadBalancingClient.OpCreateRoom(enterRoomParams);
        }
         
    }
}