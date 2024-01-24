using Core.MatchMaking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListView : MonoBehaviour
{

    private float _roomListRefreshTimer = 10;
    private RoomListCaching _roomListCaching;

    private List<GameObject> _rooms = new List<GameObject>();


    public void Init(RoomListCaching roomListCaching)
    {

        _roomListCaching = roomListCaching;
    }
    

    private void Update() 
    {

        if (_roomListCaching == null) return;
        
        if(_roomListRefreshTimer > 0)
        {
            _roomListRefreshTimer -= Time.deltaTime;
        }
        else
        {
            _roomListRefreshTimer = 10;

            Debug.ClearDeveloperConsole();

            foreach(var info in _roomListCaching._cachedRoomList)
            {
                var infoRoom = info.Value;

                Debug.Log($" <color=green> Найдена комната : </color> {infoRoom.Name}");
            }
        }
    }
}
