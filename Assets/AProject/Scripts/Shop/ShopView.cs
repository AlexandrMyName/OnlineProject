using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TadWhat.Shop
{
    public class ShopView : MonoBehaviour
    {


        [SerializeField] private Button _back;

        [SerializeField] private GameObject _gameWindow;

        [SerializeField] private GameObject _itemFab;

        [SerializeField] private List<ItemInShop> _items = new();
        [SerializeField] private Transform _conteinerView;

        [SerializeField] private TMP_Text _scText;
        [SerializeField] private TMP_Text _goldText;

        int _myCurrencySC;
         
        public void Init()
        {

            UpdateCurrentCurrencyInventory();
            int goldInMemory = 0;
            _gameWindow.SetActive(false);
            this.gameObject.SetActive(true);
        
            if (!PlayerPrefs.HasKey("twGold"))
            {
                PlayerPrefs.SetInt("twGold", 0);
            }
            else
            {
                goldInMemory = PlayerPrefs.GetInt("twGold");
                
            }

            _goldText.text = $"GOLD: <color=green>{goldInMemory}</color>";

            PlayFabClientAPI.GetCatalogItems(new PlayFab.ClientModels.GetCatalogItemsRequest(),
                res =>
                {
                    foreach(var item in res.Catalog)
                    {
                        Debug.LogWarning($"Item name : <color=red>{item.ItemId}</color> \n" +
                            $"Price (SC):  <color=red>{item.VirtualCurrencyPrices["SC"]}</color>");

                        var objectInstance = Instantiate(_itemFab,_conteinerView);
                        var button = objectInstance.GetComponent<Button>();
                        var textName = objectInstance.transform.GetChild(0).GetComponent<TMP_Text>();
                        var textPrice = objectInstance.transform.GetChild(1).GetComponent<TMP_Text>();

                        textPrice.text = $"Price (SC):  <color=red>{item.VirtualCurrencyPrices["SC"]}</color>";
                        textName.text = $"Item name : <color=red>{item.ItemId}</color>";
                        button.interactable = _myCurrencySC >= item.VirtualCurrencyPrices["SC"];

                        var itemStack = new ItemInShop()
                        {
                            OnItemClicked = button,
                            Price = (int)item.VirtualCurrencyPrices["SC"],
                            Value = item.ItemId == "Gold_id" ? 100 : 30,
                            Instance = objectInstance
                        };

                        button.onClick.AddListener(() =>
                        {
                            if(_myCurrencySC >= item.VirtualCurrencyPrices["SC"])
                            {
                                _myCurrencySC -= (int)item.VirtualCurrencyPrices["SC"];
                                UpdateCurrentCurrencyInventory((int)item.VirtualCurrencyPrices["SC"]);
                                goldInMemory = PlayerPrefs.GetInt("twGold");
                                goldInMemory += itemStack.Value;
                                PlayerPrefs.SetInt("twGold", goldInMemory);
                                _goldText.text = $"GOLD: <color=green>{goldInMemory}</color>";

                            }

                            button.interactable = _myCurrencySC >= item.VirtualCurrencyPrices["SC"];
                        });
                       
                        _items.Add(itemStack);
                    }
                    
                },
                err =>
                {
                    Debug.LogWarning(err.GenerateErrorReport());
                });

            _back.onClick.AddListener(() =>
            {
                _gameWindow.SetActive(true);
                this.gameObject.SetActive(false);

                foreach (var item in _items)
                {
                    Destroy(item.Instance);

                }
                _items.Clear();
            });
        }

        private void UpdateCurrentCurrencyInventory(int minus = 0)
        {
            PlayFabClientAPI.GetUserInventory(new PlayFab.ClientModels.GetUserInventoryRequest(), res =>
            {
                if(minus > 0)
                {
                    res.VirtualCurrency["SC"] -= minus;
                    PlayFabClientAPI.SubtractUserVirtualCurrency(new PlayFab.ClientModels.SubtractUserVirtualCurrencyRequest()
                    {
                        Amount = minus,
                        VirtualCurrency = "SC",
                        
                    }, res =>
                    {
                        Debug.Log("Данные <color=red>(SC)</color> изменены");
                    }, err =>
                    {
                        Debug.Log("Данные <color=red>(SC)</color> не изменены " + err.GenerateErrorReport());
                    });
                }
                _myCurrencySC = res.VirtualCurrency["SC"];
                _scText.text = $"My soft currency : <color=green>{res.VirtualCurrency["SC"]}</color>";


            }, err =>
            {
                Debug.LogError("Получение игровой валюты - сбой!");
            });

            
        }


        private void OnDestroy()
        {
            _back.onClick.RemoveAllListeners();
            
        }

    }

    [Serializable]
    public class ItemInShop
    {

        public int Price { get; set; }  
        public int Value { get; set; }
        public Button OnItemClicked { get; set; }
        public GameObject Instance { get; set; }
    }
}