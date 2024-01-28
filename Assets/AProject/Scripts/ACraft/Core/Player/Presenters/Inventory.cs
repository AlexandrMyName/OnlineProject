using Core.Models;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TadWhat.ACraft
{

    public class Inventory : MonoBehaviour
    {

        [SerializeField] private Button _onCloseButton;
        [SerializeField] private FreeFlyCamera _flyCamera;
        [SerializeField] private GameObject _itemViewFab;
        [SerializeField] private Transform _context;

        [SerializeField] private IconsConfigs _iconsCnf;
         

        private List<InventoryItemView> _items;
        private InventoryBar _bar;
         
        private bool _isActive;


        public void Init(InventoryBar bar)
        {

            Debug.Log("<color=green> Init Inventory </color>");

            _items ??= new List<InventoryItemView>();
            _bar = bar;

            _onCloseButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
        }


        public void InitCreativeInventory(BlockType[] blocks )
        {

            _items ??= new List<InventoryItemView>();
            foreach ( var block in blocks)
            {
                var instance = Instantiate(_itemViewFab, _context);
                var itemView = instance.GetOrAddComponent<InventoryItemView>();
                var item = _iconsCnf.Configs.Find(icon => icon.Type == block);
                if(item != null) 
                    itemView.Init(item.Icon, block, OnItemClicked);
                else
                {
                    if (itemView == null) Debug.Log("Null");
                    if (item == null) continue;
                       
                }
                _items.Add(itemView);
            }
            
        }


        private void OnItemClicked(BlockType type)
        {

            _bar.ChangeBlock(type);
            
        }

        public void RunInventory()
        {

            if (Input.GetKeyDown(KeyCode.R))
            {

                _isActive = !_isActive;

                gameObject.SetActive(_isActive);

                if (_isActive)
                {
                    Cursor.lockState = CursorLockMode.None;
                    
                }else Cursor.lockState = CursorLockMode.Locked;

                _flyCamera.enabled = !_isActive;
                Cursor.visible = _isActive;
            }
        }

        private void OnDestroy()
        {
            _onCloseButton.onClick.RemoveAllListeners();
        }
    }
}