using UnityEngine;
using Core.Models;
using System.Collections.Generic;
using JetBrains.Annotations;

public class InventoryBar 
{

    public Dictionary<int, BlockType> BlocksInBar = new Dictionary<int, BlockType>();

    private InventoryBarView _view;
    private IconsConfigs _model;
    private int _defaultItem;

    public void LoadDataBar(List<BlockType> blocks, [NotNull] InventoryBarView view,[NotNull] IconsConfigs model)
    {
        _view = view;
        _model = model;

        int itemNumber;

        for(itemNumber = 1; itemNumber <= 9; itemNumber++)
        {
            Debug.LogWarning(itemNumber);
            BlocksInBar.Add(itemNumber, blocks[itemNumber - 1]);
            _view.InitItem(itemNumber, _model.Configs.Find(item => item.Type == blocks[itemNumber - 1]));
        }
    }

    
    public void RunInventoryButtons()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1 ))
        {
             CurrentBlock.CurrentCraftingBlock = BlocksInBar[1];
            _defaultItem = 1;
            _view.SetDefaultItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[2];
            _defaultItem = 2;
            _view.SetDefaultItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[3];
            _defaultItem = 3;
            _view.SetDefaultItem(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[4];
            _defaultItem = 4;
            _view.SetDefaultItem(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[5];
            _defaultItem = 5;
            _view.SetDefaultItem(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[6];
            _defaultItem = 6;
            _view.SetDefaultItem(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[7];
            _defaultItem = 7;
            _view.SetDefaultItem(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[8];
            _defaultItem = 8;
            _view.SetDefaultItem(8);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[9];
            _defaultItem = 9;
            _view.SetDefaultItem(9);
        }

    }
}
