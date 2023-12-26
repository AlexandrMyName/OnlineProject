using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;


namespace Online.Login
{

    public class LogIN : MonoBehaviour
    {


        void Start()
        {

            if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
            {
                PlayFabSettings.TitleId = "6D22C";
            }


            var request = new LoginWithCustomIDRequest()
            {
                CustomId = "Player 1",
                CreateAccount = true,
            };


            PlayFabClientAPI.LoginWithCustomID(request,

                req =>
                {
                    Debug.Log("Complete!");
                }, error =>
                {
                    Debug.Log("Error!" + error.GenerateErrorReport());
                });
        }

    }
}