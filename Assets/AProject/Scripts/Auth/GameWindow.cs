using PlayFab;
using TadWhat.EnterView;
using TadWhat.Shop;
using UnityEngine;
using UnityEngine.UI;


public class GameWindow : MonoBehaviour
{

    [SerializeField] private Button _removeProfileInfo;
    [SerializeField] private Button _shopButton;

    [SerializeField] private EnterInGameView _enterInGameView;

    [SerializeField] private ShopView _shopObject;

  
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
    }
 

    private void OnDestroy()
    {
        _removeProfileInfo.onClick.RemoveAllListeners();
        _shopButton.onClick.RemoveAllListeners();
    }
}
