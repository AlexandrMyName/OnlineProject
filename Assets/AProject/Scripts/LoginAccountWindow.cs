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
using System.Text;
using Cryptograph.Xml;

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

        private void OnEnable()
        {
            if(_userName.Length > 0)
            {
                _nameField.text = _userName;
            }
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


        private void OnDisable()
        {
            _successText.text = "";
            _warningText.text = "";
        }


        public void LogIN()
        {

            if (_loginRequest != null)
            {
                _loadingObject.SetActive(true);
                LogInWithRequest(_loginRequest);
            }
            else
            {
                _loginRequest = SecurityCheckFiles();
                
            }
        }


        private void LogInWithRequest(LoginWithPlayFabRequest request)
        {

            PlayFabClientAPI.LoginWithPlayFab(request,

                res =>
                {
                    var warn = "<color=red>>Succes</color> auto login!";
                    Debug.LogWarning(warn);
                    //_warningText.text = warn;
                    _hidenObjectsOnSuccess.ForEach(obj => obj.SetActive(false));
                    _shownObjectsOnSuccess.ForEach(obj => obj.SetActive(true));
                    _successText.text = "�� ������� ����� � �������! \n" +
                    "����� ���������� " + _userName;
                    _loadingObject.SetActive(false);

                  
                        PlayerPrefs.SetString("tw_autoLogin", "true");
                },
                err =>
                {
                    Debug.LogWarning($"Err!  <color=red>{err.ErrorMessage}</color>");
                    _warningText.text = $"Err!  <color=red>{err.ErrorMessage}</color>";
                    _loadingObject.SetActive(false);
                });
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

                try
                {
                    var xml = XmlConverter.Create(fullPath);
                    var secret = xml.Load(new XmlSecrets(), "xml");


                    if(_password != secret.Password || _userName != secret.User_Name)
                    {
                        string warn = "<color=red>������ ��������� ������ �� ���������</color> \n" +
                       "��������� ������ ��� ��� ������������";

                        _warningText.text = warn;

                        return null;
                    }

                    shaID = secret.SHA_publicKey;
                    salt = secret.Salt_Key;

                    _password = secret.Password;
                    _userName = secret.User_Name;

                    PlayerPrefs.SetString("salt_KEY",secret.Salt_Key);
                    PlayerPrefs.SetString("sha_KEY",secret.SHA_publicKey);
                }
                 catch
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