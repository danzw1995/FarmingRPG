using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuInventoryManagement : MonoBehaviour
{
    [SerializeField] private PauseMenuInventoryManagementSlot[] inventoryManagementSlots = null;

    public GameObject inventoryManagementDraggedItemPrefab;

    [SerializeField] private Sprite transparent16x16 = null;

    [HideInInspector] public GameObject inventoryTextBoxGameObject;

    private void OnEnable()
    {
        EventHandler.InventoryUpdateEvent += PopulatePlayerInventory;

        if (InventoryManager.Instance != null)
        {
            PopulatePlayerInventory(InventoryLocation.player, InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player]);
        }
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdateEvent -= PopulatePlayerInventory;

        DestroyInventoryTextGameObject();

    }

    private void PopulatePlayerInventory(InventoryLocation inventoryLocation, List<InventoryItem> inventoryItems)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            InitialiseInventoryManagementSlots();

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(inventoryItems[i].itemCode);

                if (itemDetails != null)
                {
                    inventoryManagementSlots[i].itemDetails = itemDetails;
                    inventoryManagementSlots[i].itemQuantity = inventoryItems[i].itemQuantity;

                    inventoryManagementSlots[i].inventoryManagementSlotImage.sprite = itemDetails.itemSprite;
                    inventoryManagementSlots[i].textMeshPro.text = inventoryItems[i].itemQuantity.ToString();
                }
            }
        }
    }

    private void InitialiseInventoryManagementSlots ()
    {
        for (int i = 0; i < inventoryManagementSlots.Length; i ++)
        {
            inventoryManagementSlots[i].itemDetails = null;
            inventoryManagementSlots[i].itemQuantity = 0;

            inventoryManagementSlots[i].greyedOutImageGO.SetActive(false);
            inventoryManagementSlots[i].inventoryManagementSlotImage.sprite = transparent16x16;
            inventoryManagementSlots[i].textMeshPro.text = "";
        }

        for (int i = InventoryManager.Instance.inventoryListCapacityArray[(int)InventoryLocation.player]; i < Settings.playerMaximumInventoryCapacity;  i ++)
        {
            inventoryManagementSlots[i].greyedOutImageGO.SetActive(true);
        }
    }

    public void DestroyInventoryTextGameObject()
    {
        if (inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryTextBoxGameObject);

        }
    }

    public void DestroyCurrentlyDraggedItem()
    {
        List<InventoryItem> playerInventoryItems = InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player];
        for (int i = 0; i < playerInventoryItems.Count; i ++)
        {
            if (inventoryManagementSlots[i].draggedItem != null)
            {
                Destroy(inventoryManagementSlots[i].draggedItem);
            }
        }
    }




}
