using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private Camera mainCamera;
    private Transform parentTransform;
    public GameObject draggedItem;

    private GridCursor gridCursor;
    private Cursor cursor;

    private Canvas parentCanvas;

    public Image inventorySlotHighlight;
    public Image inventoryImage;

    public TextMeshProUGUI textMeshProUGUI;


    [SerializeField] UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;

    // 物品明细
    [HideInInspector] public ItemDetails itemDetails;
    
    // 是否选中
    [HideInInspector] public bool isSelected;
    [SerializeField] private GameObject itemPrefab = null;

    // 物品数量
    [HideInInspector] public int itemQuantity;

    // 背包栏编号
    [SerializeField] public int slotNumber;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        mainCamera = Camera.main;

        gridCursor = GameObject.FindObjectOfType<GridCursor>();
        cursor = GameObject.FindObjectOfType<Cursor>();

    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoad;
        EventHandler.DropSelectItemEvent += DropSelectedItemAndMousePosition;
        EventHandler.RemoveSelectedItemFormInventoryEvent += RemoveSelectedItemFormInventory;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoad;
        EventHandler.DropSelectItemEvent -= DropSelectedItemAndMousePosition;
        EventHandler.RemoveSelectedItemFormInventoryEvent -= RemoveSelectedItemFormInventory;

    }

    private void ClearCursors()
    {
        gridCursor.DisableCursor();
        gridCursor.selectedItemType = ItemType.none;

        cursor.DisableCursor();
        cursor.selectedItemType = ItemType.none;
    }

    public void SceneLoad()
    {
        parentTransform = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails != null)
        {
            Player.Instance.DisablePlayerInput();

            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);


            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();

            draggedItemImage.sprite = inventoryImage.sprite;

            SetSelectedItem();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            Destroy(draggedItem);

            GameObject tempObject = eventData.pointerCurrentRaycast.gameObject;


            if (tempObject != null && tempObject.transform.parent.gameObject.GetComponent<UIInventorySlot>() != null)
            {
               int toSlotNumber = tempObject.transform.parent.gameObject.GetComponent<UIInventorySlot>().slotNumber;

                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);
                ClearSelectedItem();
            } else
            {
                if (itemDetails.canBeDropped)
                {
                    DropSelectedItemAndMousePosition();
                }
            }
        }

        Player.Instance.EnablePlayerInput();
    }

    /// <summary>
    /// 丢弃选中的物品至鼠标位置
    /// </summary>
    private void DropSelectedItemAndMousePosition()
    {
        if (itemDetails != null && isSelected)  
        {
            if (gridCursor.cursorPositionIsValid)
            {
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

                Vector3Int gridPosition = GridPropertiesManager.Instance.grid.WorldToCell(worldPosition);

                GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x, worldPosition.y - Settings.gridCellSize / 2, worldPosition.z), Quaternion.identity, parentTransform);

                Item item = itemGameObject.GetComponent<Item>();
                item.itemCode = itemDetails.itemCode;

                InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.itemCode);

                if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.itemCode) == -1)
                {
                    ClearSelectedItem();
                }

            }



        }
    }

    private void RemoveSelectedItemFormInventory()
    {
        if (itemDetails != null && isSelected)
        {
            int itemCode = itemDetails.itemCode;

            InventoryManager.Instance.RemoveItem(InventoryLocation.player, itemCode);

            if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, itemCode) == -1)
            {
                ClearSelectedItem();
            }

        }
    }
    /// <summary>
    /// 鼠标移入物品栏，显示物品的描述信息
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemQuantity > 0)
        {
            inventoryBar.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            inventoryTextBox.SetTextBoxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            if (inventoryBar.isInventoryBarPositionBottom)
            {

                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 25f, transform.position.z);
            } else
            {

                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 25f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventoryBar.inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameObject);
        }
    }

    /// <summary>
    ///  选中|取消选中物品
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected)
        {
            ClearSelectedItem();
        
        } else if (itemQuantity > 0) {

            SetSelectedItem();
          
        }
    }

    public void ClearSelectedItem()
    {
        ClearCursors();

        inventoryBar.ClearHighlightInventorySlots();

        isSelected = false;

        InventoryManager.Instance.ClearSelectedVentoryItem(InventoryLocation.player);

        Player.Instance.ClearCarriedItem();

    }

    public void SetSelectedItem()
    {

        inventoryBar.ClearHighlightInventorySlots();

        isSelected = true;


        inventoryBar.SetHighlightInventorySlots();

        gridCursor.itemUseGridRadius = itemDetails.itemUseGridRadius;
        cursor.itemUseRadius = itemDetails.itemUseRadius;

        if (itemDetails.itemUseGridRadius > 0)
        {
            gridCursor.EnableCursor();
        } else
        {
            gridCursor.DisableCursor();
        }


        if (itemDetails.itemUseRadius > 0)
        {
            cursor.EnableCursor();
        }
        else
        {
            cursor.DisableCursor();
        }


        gridCursor.selectedItemType = itemDetails.itemType;
        cursor.selectedItemType = itemDetails.itemType;

        InventoryManager.Instance.SetSelectedVentoryItem(InventoryLocation.player, itemDetails.itemCode);

        if (itemDetails.canBeCarried)
        {
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);
        } else
        {
            Player.Instance.ClearCarriedItem();

        }
    }
}
