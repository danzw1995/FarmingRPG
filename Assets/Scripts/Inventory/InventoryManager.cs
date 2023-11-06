using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonoBehaviour<InventoryManager>, ISaveable
{
    private UIInventoryBar inventoryBar;
    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    private int[] selectedVentoryItems;

    public List<InventoryItem>[] inventoryLists;

    [HideInInspector]
    public int[] inventoryListCapacityArray;


    [SerializeField]
    private SO_ItemList itemList = null;


    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }
    private GameObjectSave _gameObjectSave;
    public GameObjectSave gameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGuid>().GUID;

        gameObjectSave = new GameObjectSave();


        CreateInventoryLists();

        CreateItemDetailDictionary();

        selectedVentoryItems = new int[(int)InventoryLocation.count];

        for (int i = 0; i < (int)InventoryLocation.count; i ++)
        {
            selectedVentoryItems[i] = -1;
        }
    }

    private void Start()
    {
        inventoryBar = FindObjectOfType<UIInventoryBar>();
    }

    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
    }

    private void CreateInventoryLists()
    {
        inventoryLists = new  List<InventoryItem>[(int)InventoryLocation.count];

        for(int i = 0; i < (int)InventoryLocation.count; i ++ )
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        inventoryListCapacityArray = new int[(int)InventoryLocation.count];

        inventoryListCapacityArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }

    private void CreateItemDetailDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();
        if (itemList != null)
        {
            foreach(ItemDetails itemDetails in itemList.itemDetails)
            {
                itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
            }
        }
    }

    public string GetItemTypeDescription(ItemType itemType)
    {
        string description = "";
        switch (itemType) {
            case ItemType.Hoeing_tool:
                description = Settings.HoeingTool;
                break;
            case ItemType.Chopping_tool:
                description = Settings.ChoppingTool;
                break;
            case ItemType.Breaking_tool:
                description = Settings.BreakingTool;
                break;
            case ItemType.Reaping_tool:
                description = Settings.ReapingTool;
                break;
            case ItemType.Watering_tool:
                description = Settings.WateringTool;
                break;
            default:
                description = itemType.ToString();
                break;

        }
        return description;
    }

    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);

        Destroy(gameObjectToDelete);
    }

    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.itemCode;

        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        int position = FindItemInInventory(inventoryLocation, itemCode);

        if (position != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, position);
        } else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryList);
    }

    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {

        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        int position = FindItemInInventory(inventoryLocation, itemCode);

        if (position != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, position);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryList);
    }

    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        int posisiton = FindItemInInventory(inventoryLocation, itemCode);
        if (posisiton > -1)
        {
            RemoveItemAtPosition(inventoryList, itemCode, posisiton);
        }
        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryList);
    }

    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = inventoryList[position];
        if (inventoryItem.itemQuantity > 1)
        {
            inventoryItem.itemQuantity -= 1;

            inventoryList[position] = inventoryItem;
        } else
        {
            inventoryList.RemoveAt(position);
        }


    }

    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem
        {
            itemCode = itemCode,
            itemQuantity = 1
        };

        inventoryList.Add(inventoryItem);

        //DebugPrintInventoryList(inventoryList);

    }

   /* private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    {
        Debug.Log("*************背包数据开始******************");
        foreach(InventoryItem inventoryItem in inventoryList)
        {
            Debug.Log("Item Description:" + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + ", ItemQuantity: " + inventoryItem.itemQuantity);
        }

        Debug.Log("**************背包数据结束****************");
    }*/

    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = inventoryList[position];

        InventoryItem newInventoryItem = new InventoryItem
        {
            itemCode = itemCode,
            itemQuantity = inventoryItem.itemQuantity + 1
        };

        inventoryList[position] = newInventoryItem;

        Debug.ClearDeveloperConsole();

      //  DebugPrintInventoryList(inventoryList);

    }

    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromSlotNumber, int toSlotNumber)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        if (fromSlotNumber < inventoryList.Count && toSlotNumber < inventoryList.Count && fromSlotNumber >= 0 && toSlotNumber >= 0)
        {
            InventoryItem fromInvetoryItem = inventoryList[fromSlotNumber];

            inventoryList[fromSlotNumber] = inventoryList[toSlotNumber];

            inventoryList[toSlotNumber] = fromInvetoryItem;
        }
        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryList);


    }

    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {

        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];
        for (int i = 0; i < inventoryList.Count; i ++)
        {
            if (inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }
        return -1;
    }

    public ItemDetails GetItemDetails(int itemCode) {
        ItemDetails itemDetails;

        if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        } else
        {
            return null;
        }
    }

    public void SetSelectedVentoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedVentoryItems[(int)inventoryLocation] = itemCode;
    }

    public void ClearSelectedVentoryItem(InventoryLocation inventoryLocation)
    {
        selectedVentoryItems[(int)inventoryLocation] = -1;
    }


    public int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedVentoryItems[(int)inventoryLocation];
    }

    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);
        if (itemCode == -1)
        {
            return null;
        }
        return GetItemDetails(itemCode);
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        SceneSave sceneSave = new SceneSave();

        gameObjectSave.sceneData.Remove(Settings.persistentScene);

        sceneSave.inventoryItemLists = inventoryLists;

        sceneSave.intArrayDictionary = new Dictionary<string, int[]>();

        sceneSave.intArrayDictionary.Add("inventoryListCapacityArray", inventoryListCapacityArray);

        gameObjectSave.sceneData.Add(Settings.persistentScene, sceneSave);

        return gameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameSaveData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            this.gameObjectSave = gameObjectSave;

            if (gameObjectSave.sceneData.TryGetValue(Settings.persistentScene, out SceneSave sceneSave))
            {
                if (sceneSave.inventoryItemLists != null)
                {
                    inventoryLists = sceneSave.inventoryItemLists;

                    for (int i = 0; i < (int)InventoryLocation.count; i++)
                    {
                        EventHandler.CallInventoryUpdateEvent((InventoryLocation)i, inventoryLists[i]);
                    }

                    Player.Instance.ClearCarriedItem();

                    inventoryBar.ClearHighlightInventorySlots();
                }

                if (sceneSave.intArrayDictionary != null && sceneSave.intArrayDictionary.TryGetValue("inventoryListCapacityArray", out int[] inventoryListCapacityArray))
                {
                    this.inventoryListCapacityArray = inventoryListCapacityArray;
                }
              
            }
        }
    }

    public void ISaveableStoreScene(string sceneName) { }

    public void ISaveableReStoreScene(string sceneName) { }


}
