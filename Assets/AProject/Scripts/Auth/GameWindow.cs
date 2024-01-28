using Core.MatchMaking;
using PlayFab;
using System.Collections;
using System.IO;
using TadWhat.ACraft.Constructor;
using TadWhat.Auth;
using TadWhat.EnterView;
using TadWhat.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameWindow : MonoBehaviour
{

    [SerializeField] private bool _isEditModeForAdmin;
    [SerializeField] private Button _removeProfileInfo;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _adminAPI_button;
    [SerializeField] private Button _lobby_button;
    [SerializeField] private AdminView _adminEditorView;

    [SerializeField] private EnterInGameView _enterInGameView;
   
    [SerializeField] private ShopView _shopObject;
    [SerializeField] private TMP_Text _adminInformation;


    [SerializeField] private EditChunckAndCreation _chunckEditor;


    void Start()
    {
 
         
        _removeProfileInfo.onClick.AddListener(() =>
        {
            if (PlayerPrefs.HasKey("tw_autoLogin"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }

            PlayFabClientAPI.ForgetAllCredentials();
            _enterInGameView.ChangeView(TadWhat.TypeOfButton.LogIN);
            this.gameObject.SetActive(false);
        });

        _shopButton.onClick.AddListener(() =>
        {

            _shopObject.Init();
        });

        if (_isEditModeForAdmin)
        {
            _adminAPI_button.interactable = true;

            _adminInformation.text = $"";

            _adminAPI_button.onClick.AddListener(() =>
            {
                
                _adminEditorView.Init();
                this.gameObject.SetActive(false);
                _adminEditorView.gameObject.SetActive(true);
            });
        }
        else
        {
            _adminAPI_button.interactable = false;
            _adminInformation.text = $"<color=red>права администратора не доступны</color>";
        }
    }

     

    private void OnDestroy()
    {
        _removeProfileInfo.onClick.RemoveAllListeners();
        _shopButton.onClick.RemoveAllListeners();
    }
}
