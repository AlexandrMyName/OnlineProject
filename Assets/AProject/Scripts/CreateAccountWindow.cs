using Cryptograph;
using PlayFab;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
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


        private void Start()
        {

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
                var user = ProtectorAES.Register(_userName, _password);

                _loadingObject.SetActive(true);
                PlayFabClientAPI.RegisterPlayFabUser(new PlayFab.ClientModels.RegisterPlayFabUserRequest()
                {
                    Email = _email,
                    Password = user.SaltedHashedPassword,
                    Username = user.Name,
                      
                }, res =>
                {
                    _hidenObjectsOnSuccess.ForEach(obj => obj.SetActive(false));
                    _shownObjectsOnSuccess.ForEach(obj => obj.SetActive(true));

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

                    XmlSerializer xml = new XmlSerializer(typeof(XmlSecrets));

                    
                    using (var streamData = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (var brottlyAl = new BrotliStream(streamData,CompressionMode.Compress))
                            xml.Serialize(streamData, secret);
                    }

                    Debug.LogWarning("Success created account!");

                    _resultText.text = $"Success created account! \n" +
                    $" You'r Salt and SHA public key saved in: \n {fullPath}  \n" +
                    $"If you wonna change a device, you need copy this file and go to game folder" ;

                     
                    PlayerPrefs.SetString("salt_KEY", user.Salt);
                    PlayerPrefs.SetString("publicSHA_KEY", ProtectorAES.PublicKey);
                    PlayerPrefs.SetString("tw_password", _password);
                    PlayerPrefs.SetString("tw_userName", _userName);
                    PlayerPrefs.SetString("tw_autoLogin", "true");

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