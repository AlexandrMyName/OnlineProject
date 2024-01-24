using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;


namespace Core.MatchMaking
{

    public abstract class BaseRoomMatchMaker : IMatchmakingCallbacks
    {

        protected LoadBalancingClient LoadBalancingClient;


        public abstract void FindQuickGame();


        protected abstract void CreateRoom(string name, byte maxPlayersInRoom = 4);
          

        public void OnCreatedRoom()
        {
            Debug.Log($"<color=green>[OnCreatedRoom]</color> ");
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log($"<color=red>[OnCreateRoomFailed]</color> {message}");
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            Debug.Log($"<color=green>[OnFriendListUpdate]</color> ");
        }

        public void OnJoinedRoom()
        {
            Debug.Log($"<color=green>[OnJoinedRoom]</color> ");
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {

            Debug.Log($"<color=red>[OnJoinRandomFailed]</color> {message}");
          
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"<color=red>[OnJoinRoomFailed]</color> {message}");
        }

        public void OnLeftRoom()
        {
            Debug.Log($"<color=blue>[OnLeftRoom]</color> ");
        }
    }
}