using Cryptograph;
using Cryptograph.Xml;
using PlayFab;
using PlayFab.EconomyModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace TadWhat.CreateAccountView
{

    public class CreateAccountWindow : MonoBehaviour
    {
         
        [SerializeField] private TMP_InputField _userNameInput;
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _passwordInput;

        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _back_button;
        [SerializeField] private TMP_Text _resultText;
        [SerializeField] private GameObject _loadingObject;

        private string _userName;
        private string _password;
        private string _email;

        [SerializeField] private List<GameObject> _hidenObjectsOnSuccess;

        [SerializeField] private List<GameObject> _shownObjectsOnSuccess;

        [SerializeField] private GameObject _enterInGameWindow;

        public static bool USE_ENCRYPT;


        private void Start()
        {

            USE_ENCRYPT = false;

            _userNameInput.onValueChanged.AddListener(value => { _userName = value; });

            _emailInput.onValueChanged.AddListener(value => { _email = value; });

            _passwordInput.onValueChanged.AddListener(value => { _password = value; });

            _back_button.onClick.AddListener(() =>
            {
                _enterInGameWindow.SetActive(true);
                this.gameObject.SetActive(false);
            });

            _acceptButton.onClick.AddListener(() =>
            {

                _resultText.text = "";
                User user;

                if (USE_ENCRYPT)
                     user = ProtectorAES.Register(_userName, _password);
                else
                {
                    user = new User()
                    {
                        Name = _userName,
                        SaltedHashedPassword = _password,
                        Salt = ""
                    };
                }

                _loadingObject.SetActive(true);

                if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
                {
                    PlayFabSettings.staticSettings.TitleId = " A823B";
                }

                PlayFabClientAPI.RegisterPlayFabUser(new PlayFab.ClientModels.RegisterPlayFabUserRequest()
                {
                    Email = _email,
                    Password = user.SaltedHashedPassword,
                    Username = user.Name,
                    DisplayName = _userName,  
                      
                }, res =>
                {
                    _hidenObjectsOnSuccess.ForEach(obj => obj.SetActive(false));
                    _shownObjectsOnSuccess.ForEach(obj => obj.SetActive(true));

                    Debug.LogWarning("Success created account!");

                    if (USE_ENCRYPT)
                    {
                        var path = Application.dataPath;
                        var fullPath = Path.Combine(path, "secret.xml");
                        _loadingObject.SetActive(false);
                        XmlSecrets secret = new XmlSecrets()
                        {
                            Salt_Key = user.Salt,
                            SHA_publicKey = ProtectorAES.PublicKey,
                            Password = _password,
                            User_Name = _userName
                        };

                        var xml = XmlConverter.Create(fullPath);
                        xml.Save(secret, "xml");

                        

                        _resultText.text = $"Success created account! \n" +
                        $" You'r Salt and SHA public key saved in: \n {fullPath}  \n" +
                        $"If you wonna change a device, you need copy this file and go to game folder";


                        PlayerPrefs.SetString("salt_KEY", user.Salt);
                        PlayerPrefs.SetString("publicSHA_KEY", ProtectorAES.PublicKey);
                        PlayerPrefs.SetString("tw_password", _password);
                        PlayerPrefs.SetString("tw_userName", _userName);
                        PlayerPrefs.SetString("tw_autoLogin", "true");
                        PlayerPrefs.SetString("PlayFabID",  res.PlayFabId);//To xml
                    }

                    PlayFabServerAPI.GrantItemsToUser(new PlayFab.ServerModels.GrantItemsToUserRequest() {
                          Annotation = "" ,
                          ItemIds = new List<string> { "Character_token" } ,
                          PlayFabId = res.PlayFabId,
                          CustomTags = new Dictionary<string, string> { { "character", " token" } }
                    }, resToken =>
                    {
                        
                        Debug.Log($"<color=green>ВЫДАН ТОКЕН СОЗДАНИЯ ПЕРСОНАЖА</color>");

                        Debug.Log($"<color=yellow>Создание персонажа</color>");

                            PlayFabServerAPI.GrantCharacterToUser(new PlayFab.ServerModels.GrantCharacterToUserRequest()
                            {
                                CharacterName = "Steve(default)",
                                PlayFabId = res.PlayFabId,
                                CharacterType = "default",
                                CustomTags = new Dictionary<string, string>()
                                {
                                    {"Level","0"},
                                    {"Exp","0"},
                                    {"MaxHealth","100"},
                                }
                            }, resultToCreateCharacter =>
                            {
                                Debug.Log($"<color=green>Персонаж успешно создан!</color> Имя скина: <color=red>Steve</color>");
                                SceneManager.LoadScene(0);
                                
                            }, errToCreateCharacter =>
                            {  
                                Debug.Log($"<color=red>ошибка создания персонажа {errToCreateCharacter.GenerateErrorReport()}</color>");
                            });

                    }, errToken =>
                    {
                        Debug.Log($"<color=red>ошибка выдачи токена создания персонажа {errToken.GenerateErrorReport()}</color>");
                    });

                }, err =>
                {
                    _loadingObject.SetActive(false);
                    Debug.LogWarning($"Err!  <color=red>{err.ErrorMessage}</color>");
                    _resultText.text = $"Err!  <color=red>{err.ErrorMessage}</color>";

                    if(PlayerPrefs.HasKey("salt_KEY"))
                        PlayerPrefs.DeleteKey("salt_KEY");
                    if (PlayerPrefs.HasKey("publicSHA_KEY"))
                        PlayerPrefs.DeleteKey("publicSHA_KEY");
                    if (PlayerPrefs.HasKey("tw_autoLogin"))
                        PlayerPrefs.DeleteKey("tw_autoLogin");
                });
            });
        }
        private void OnDisable()
        {
            _resultText.text = "";
        }



        private void OnDestroy()
        {
            _acceptButton.onClick.RemoveAllListeners();

            _back_button.onClick.RemoveAllListeners();
        }
    }
}