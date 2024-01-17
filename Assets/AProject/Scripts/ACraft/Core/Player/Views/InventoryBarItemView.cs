using UnityEngine;
using UnityEngine.UI;



public class InventoryBarItemView : MonoBehaviour
{

    [SerializeField] private Image Icon;
    [SerializeField] private Image Bird;

    public int Key;

    public void SetIcon(IconsConfig config)
    {
       
             Icon.sprite = config.Icon;
    }

    public void SetDefault(bool isDefault)
    {
        Bird.gameObject.SetActive(isDefault);
    }


}
