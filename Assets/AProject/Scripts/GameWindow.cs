using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{

    [SerializeField] private Button _removeAccInfo;

    void Start()
    {
        _removeAccInfo.onClick.AddListener(() =>
        {
          //  PlayFabClientAPI.Remo
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
