using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TadWhat.EnterView;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{

    [SerializeField] private Button _removeProfileInfo;
    [SerializeField] private EnterInGameView _enterInGameView;


    void Start()
    {
        _removeProfileInfo.onClick.AddListener(() =>
        {
            if (PlayerPrefs.HasKey("tw_autoLogin"))
                PlayerPrefs.DeleteKey("tw_autoLogin");

            PlayFabClientAPI.ForgetAllCredentials();
            _enterInGameView.ChangeView(TadWhat.TypeOfButton.LogIN);
            this.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
