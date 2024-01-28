using UnityEngine;
using Core.Models;
using System.Collections.Generic;
using JetBrains.Annotations;

public class InventoryBar 
{

    public Dictionary<int, BlockType> BlocksInBar = new Dictionary<int, BlockType>();

    private InventoryBarView _view;
    private IconsConfigs _model;
    public int DefaultItem;

    public void LoadDataBar(List<BlockType> blocks, [NotNull] InventoryBarView view,[NotNull] IconsConfigs model)
    {
        _view = view;
        _model = model;

        int itemNumber;

        for(itemNumber = 1; itemNumber <= 9; itemNumber++)
        {
           
            BlocksInBar.Add(itemNumber, blocks[itemNumber - 1]);
            _view.InitItem(itemNumber, _model.Configs.Find(item => item.Type == blocks[itemNumber - 1]));
        }
    }


    public void ChangeBlock(BlockType block)
    {
        BlocksInBar[DefaultItem] = block;
        CurrentBlock.CurrentCraftingBlock = BlocksInBar[DefaultItem];
    }
    
    public void RunInventoryButtons()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1 ))
        {
             CurrentBlock.CurrentCraftingBlock = BlocksInBar[1];
            DefaultItem = 1;
            _view.SetDefaultItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[2];
            DefaultItem = 2;
            _view.SetDefaultItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[3];
            DefaultItem = 3;
            _view.SetDefaultItem(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[4];
            DefaultItem = 4;
            _view.SetDefaultItem(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[5];
            DefaultItem = 5;
            _view.SetDefaultItem(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[6];
            DefaultItem = 6;
            _view.SetDefaultItem(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[7];
            DefaultItem = 7;
            _view.SetDefaultItem(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[8];
            DefaultItem = 8;
            _view.SetDefaultItem(8);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            CurrentBlock.CurrentCraftingBlock = BlocksInBar[9];
            DefaultItem = 9;
            _view.SetDefaultItem(9);
        }

    }
}
