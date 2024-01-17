using System.Collections.Generic;
using UnityEngine;



public class InventoryBarView : MonoBehaviour
{
    public List<InventoryBarItemView> Items;


    public void InitItem(int itemNumber, IconsConfig config)
    {
        if(itemNumber > 0 && itemNumber <= Items.Count)
        {
            Items.Find(item=> item.Key == itemNumber).SetIcon(config);
        }
    }

    public void SetDefaultItem(int defaultItemNumber)
    {
        for(int i = 0; i< Items.Count; i++)
        {
           
                Items[i].SetDefault(false);
             
        }
        Items[defaultItemNumber - 1].SetDefault(true);
    }
}
