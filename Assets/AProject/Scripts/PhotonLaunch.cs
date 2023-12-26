using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLaunch : MonoBehaviourPunCallbacks
{
   

    void Awake()
    {

        PhotonNetwork.AutomaticallySyncScene = true;

    }

    private void Start()
    {

        if (PhotonNetwork.IsConnected)
        {

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = Application.version;


        }
    }


    public override void OnConnectedToMaster()
    {

        base.OnConnectedToMaster();

        Debug.Log("YES!");
    }
}
