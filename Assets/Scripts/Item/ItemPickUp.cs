using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    /// <summary>
    /// 拾取物品
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();

        if (item != null)
        {
            ItemDetails itemDetails =  InventoryManager.Instance.GetItemDetails(item.itemCode);

            if (itemDetails != null && itemDetails.canBePickedUp)
            {
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);

                AudioManager.Instance.PlaySound(SoundName.effectPickupSound);
            }
        }
    }
}
