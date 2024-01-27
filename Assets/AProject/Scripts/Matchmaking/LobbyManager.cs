using Core.MatchMaking;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LobbyManager : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject _playerFab;
    [SerializeField] private Transform _spawnNet;

    private void Start()
    { 
        
        if (_playerFab == null)
        { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
             
            if (PlayerManager.LocalPlayerInstance == null)
            {

                Debug.Log(BaseRoomMatchMaker.LoadBalancingClient.State);
                 


                PhotonNetwork.Instantiate(this._playerFab.name, _spawnNet.position, Quaternion.identity, 0);

            }
            else
            {

                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
    }


     
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogWarning("Успешно вошел в комнату " + newPlayer.NickName);
        //if (PhotonNetwork.IsMasterClient)
        //    PhotonNetwork.Instantiate(this._playerFab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);

    }



}
