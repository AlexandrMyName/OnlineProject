using Cryptograph;
using Photon.Pun.Demo.Cockpit;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TadWhat.LoginAccountView
{
    public class LoginAccountWindow : MonoBehaviour
    {

        [SerializeField] private Button _logIn_button;
        [SerializeField] private Button _back_button;
        [SerializeField] private string _password;
        [SerializeField] private string _userName;

        [SerializeField] private TMP_Text _warningText;
        [SerializeField] private TMP_Text _successText;
        private LoginWithPlayFabRequest _loginRequest;

        [SerializeField] private TMP_InputField _nameField;
        [SerializeField] private TMP_InputField _passwordField;

        [SerializeField] private List<GameObject> _hidenObjectsOnSuccess;

        [SerializeField] private List<GameObject> _shownObjectsOnSuccess;


        [SerializeField] private GameObject _loadingObject;

        [SerializeField] private GameObject _enterInGameWindow;

        private void Awake()
        {

            _passwordField.onValueChanged.AddListener(value => { _password = value; });
            _nameField.onValueChanged.AddListener(value => { _userName = value; });

            _loginRequest = SecurityCheckFiles();

            if (PlayerPrefs.HasKey("tw_autoLogin"))
                LogIN();// Auto try

        }


        private void Start()
        {

            _logIn_button.onClick.AddListener(() =>
            {
                LogIN();
            });

            _back_button.onClick.AddListener(() =>
            {
                _enterInGameWindow.SetActive(true);
                this.gameObject.SetActive(false);
            });
        }


        public void LogIN()
        {

            if (_loginRequest != null)
            {
                _loadingObject.SetActive(true);
                PlayFabClientAPI.LoginWithPlayFab(_loginRequest,

                    res =>
                    {
                        var warn = "<color=red>>Succes</color> auto login!";
                        Debug.LogWarning(warn);
                        _warningText.text = warn;
                        _hidenObjectsOnSuccess.ForEach(obj => obj.SetActive(false));
                        _shownObjectsOnSuccess.ForEach(obj => obj.SetActive(true));
                        _successText.text = "Вы успешно вошли в систему! \n" +
                        "Добро пожаловать " + _userName;
                        _loadingObject.SetActive(false);

                        if (!PlayerPrefs.HasKey("tw_autoLogin"))
                            PlayerPrefs.SetString("tw_autoLogin","true");
                    },
                    err =>
                    {
                        Debug.LogWarning($"Err!  <color=red>{err.ErrorMessage}</color>");
                        _warningText.text = $"Err!  <color=red>{err.ErrorMessage}</color>";
                        _loadingObject.SetActive(false);
                    });
            }
        }


        private LoginWithPlayFabRequest SecurityCheckFiles()
        {

            var salt = "";
            var shaID = "";

            if (PlayerPrefs.HasKey("salt_KEY"))
            {

                salt = PlayerPrefs.GetString("salt_KEY");
                shaID = PlayerPrefs.GetString("sha_KEY");

                _password = PlayerPrefs.GetString("tw_password");
                _userName = PlayerPrefs.GetString("tw_userName");

            }
            else
            {
                var path = Application.dataPath;
                var fullPath = Path.Combine(path, "secret.xml");

                XmlSecrets secret;

                XmlSerializer xml = new XmlSerializer(typeof(XmlSecrets));

                if (File.Exists(fullPath))
                {
                    using (var streamData = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                    {
                        using (var brottlyAl = new BrotliStream(streamData, CompressionMode.Decompress))
                            secret = (XmlSecrets)xml.Deserialize(streamData);
                    }

                    shaID = secret.SHA_publicKey;
                    salt = secret.Salt_Key;
                    _password = secret.Password;
                    _userName = secret.User_Name;
                }
                else
                {
                    string warn = "File with secure keys not exist \n" +
                        "sha key and salt is <color=red> empty! </color>";
                    _warningText.text = warn;
                    return null;

                }
            }

            return new PlayFab.ClientModels.LoginWithPlayFabRequest()
            {

                Password = ProtectorAES.GetSaltedHashPassword(salt, _password),
                Username = _userName,
            };
        }
        private void OnDestroy()
        {
            _back_button.onClick.RemoveAllListeners();

            _logIn_button.onClick.RemoveAllListeners();
        }
    }
}