using Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemView : MonoBehaviour
{

    [SerializeField] private Image _icon;
    [SerializeField] private Button _onClickButton;
    [SerializeField] private TMP_Text _nameTxt;

    public BlockType Block { get; set; }

    public int Count { get; set; }


    private void OnValidate()
    {

        _icon ??= GetComponent<Image>();
        _onClickButton ??= GetComponent<Button>();
    }

    public void Init(Sprite icon, BlockType block, Action<BlockType> actionOnClick)
    {
        
         _nameTxt.text = block.ToString();
         _icon.sprite = icon;
         Block = block;

        _onClickButton.onClick.AddListener(() =>
        {
            actionOnClick?.Invoke(Block);
        });
    }
}
