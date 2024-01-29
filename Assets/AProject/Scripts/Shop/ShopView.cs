using PlayFab;
using PlayFab.ClientModels;
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
        [Space]
        [SerializeField] private List<ItemInShop> _items = new();
        [SerializeField] private List<ScollsItemView> _scolls = new();
        [Space]
        private Transform _conteinerView;
        [SerializeField] private Transform _conteinerCurrencies;
        [SerializeField] private Transform _conteinerCharacters;
        [SerializeField] private Transform _conteinerPrivileges;

        [SerializeField] private IconsConfigs _playerIconsConfig;

         
        [SerializeField] private TMP_Text _scText;
        [SerializeField] private TMP_Text _goldText;

        int _myCurrencySC;
         

        public void Init()
        {
             
            UpdateCurrentCurrencyInventory(0);

            var scrollViews = new List<Transform>();

            _scolls.ForEach(scroll=> scrollViews.Add(scroll.ViewTransform));
            _scolls.ForEach(scroll => scroll.Init(scrollViews));

            

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


                        switch(item.ItemClass)
                        {
                            case "Currencies":
                                {
                                    _conteinerView = _conteinerCurrencies;
                                    break;
                                }
                            case "Characters":
                                {
                                    _conteinerView = _conteinerCharacters;
                                    break;
                                }
                            case "Privileges":
                                {
                                    _conteinerView = _conteinerPrivileges;
                                    break;
                                }
                        }


                        var objectInstance = Instantiate(_itemFab, _conteinerView);
                        var button = objectInstance.GetComponent<Button>();
                        var textName = objectInstance.transform.GetChild(0).GetComponent<TMP_Text>();
                        var textPrice = objectInstance.transform.GetChild(1).GetComponent<TMP_Text>();

                        textPrice.text = $"<color=red>{item.VirtualCurrencyPrices["SC"]}</color>";
                         
                        button.interactable = _myCurrencySC >= item.VirtualCurrencyPrices["SC"];

                        ItemInShop itemStack = GetItem(item, objectInstance, button,_playerIconsConfig, textName);

                        button.onClick.AddListener(() =>
                        {
                            if (UpdateCurrentCurrencyInventory() >= item.VirtualCurrencyPrices["SC"])
                            {
                                _myCurrencySC -= (int)item.VirtualCurrencyPrices["SC"];
                                UpdateCurrentCurrencyInventory((int)item.VirtualCurrencyPrices["SC"]);
                                goldInMemory = PlayerPrefs.GetInt("twGold");
                                goldInMemory += itemStack.Value;
                                PlayerPrefs.SetInt("twGold", goldInMemory);  
                                _goldText.text = $"Золото: <color=green>{goldInMemory}</color>";

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

            _scolls[0].ViewTransform.gameObject.SetActive(true);
        }


        private static ItemInShop GetItem(CatalogItem item, GameObject objectInstance, Button button, IconsConfigs iconCnfs, TMP_Text textName)
        {
           var itemObject =  new ItemInShop();

            switch (item.ItemId) 
            {
                case "Gold_id":
                    {
                        itemObject = new ItemInShop()
                        {
                            OnItemClicked = button,
                            Price = (int)item.VirtualCurrencyPrices["SC"],
                            Value = 100,
                            Instance = objectInstance };
                        textName.text = "<color=yellow>золото</color>";
                        objectInstance.GetComponent<Image>().sprite = iconCnfs.Configs.Find(cnf => cnf.Type == Core.Models.BlockType.Player_GoldItem).Icon;
                        break;
                    }

                case "Diamond_id":
                    {
                        itemObject = new ItemInShop()
                        {
                            OnItemClicked = button,
                            Price = (int)item.VirtualCurrencyPrices["SC"],
                            Value = 10,
                            Instance = objectInstance
                        };
                        textName.text = "<color=blue>алмаз</color>";
                        objectInstance.GetComponent<Image>().sprite = iconCnfs.Configs.Find(cnf => cnf.Type == Core.Models.BlockType.Player_DiamondItem).Icon;
                        break;
                    }
                case "Emerald_id":
                    {
                        itemObject = new ItemInShop()
                        {
                            OnItemClicked = button,
                            Price = (int)item.VirtualCurrencyPrices["SC"],
                            Value = 2,
                            Instance = objectInstance
                        };
                        textName.text = "<color=green>изумруд</color>";
                        objectInstance.GetComponent<Image>().sprite = iconCnfs.Configs.Find(cnf => cnf.Type == Core.Models.BlockType.Player_EmeraldItem).Icon;
                        break;  
                    }
                case "Newt_id":
                    {
                        itemObject = new ItemInShop()
                        {
                            OnItemClicked = button,
                            Price = (int)item.VirtualCurrencyPrices["SC"],
                            Value = 1,
                            Instance = objectInstance
                        };
                        textName.text = "<color=red>тритон</color>";
                        objectInstance.GetComponent<Image>().sprite = iconCnfs.Configs.Find(cnf => cnf.Type == Core.Models.BlockType.Player_TritonItem).Icon;
                        break;  
                    }
                default:
                    {
                        itemObject = new ItemInShop()
                        {
                            OnItemClicked = button,
                            Price = 10000,
                            Value = 0,
                            Instance = objectInstance
                        };
                        break;
                    }              
            }
            return itemObject;
        }

        private int UpdateCurrentCurrencyInventory(int minus = 0)
        {
            var currency = 0;


            
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

                currency = res.VirtualCurrency["SC"];
            }, err =>
            {
                Debug.LogError("Получение игровой валюты - сбой!");
                 
            });
            return currency;
            
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

    public enum ShopItemType
    {
        Currencies,
        Characters,
        Privileges,
    }

    [Serializable]
    public class ScollsItemView : IDisposable
    {

        public Transform ViewTransform;
        public ShopItemType Type;
        public Button Button;


        public void Init(List<Transform> _views)
        {
            Button.onClick.AddListener(() =>
            {
                _views.ForEach(view => view.gameObject.SetActive(false));
                ViewTransform.gameObject.SetActive(true);
            });
        }


        public void Dispose()
        {
            Button.onClick.RemoveAllListeners();
        }
    }
}