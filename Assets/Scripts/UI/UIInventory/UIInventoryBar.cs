using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{

    [SerializeField] private Sprite blank16x16Sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;

    [HideInInspector] public GameObject inventoryBarDraggedItem;

    [HideInInspector] public GameObject inventoryTextBoxGameObject;

    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = false;

    public bool isInventoryBarPositionBottom { get { return _isInventoryBarPositionBottom; } set { _isInventoryBarPositionBottom = value; } }
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        SwitchInventoryBarPosition();
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdateEvent += InventoryUpdated;
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdateEvent -= InventoryUpdated;

    }

    /// <summary>
    /// 更新背包
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="inventoryList"></param>
    private void InventoryUpdated (InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (i < inventoryList.Count)
                {
                    InventoryItem inventoryItem = inventoryList[i];

                    ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode);

                    if (itemDetails != null)
                    {
                        inventorySlots[i].itemDetails = itemDetails;
                        inventorySlots[i].itemQuantity = inventoryItem.itemQuantity;
                        inventorySlots[i].inventoryImage.sprite = itemDetails.itemSprite;
                        inventorySlots[i].textMeshProUGUI.text = inventoryItem.itemQuantity.ToString();

                        SetHighlightInventorySlots(i);
                    }

                } else
                {
                    break;
                }
            }

        }
    }

    private void ClearInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            for(int i = 0; i < inventorySlots.Length; i ++)
            {
                inventorySlots[i].textMeshProUGUI.text = "";
                inventorySlots[i].itemDetails = null;
                inventorySlots[i].inventoryImage.sprite = blank16x16Sprite;
                inventorySlots[i].itemQuantity = 0;

                // SetHighlightInventorySlots(i);
            }
        }
    }

    /// <summary>
    /// 切换背包的位置，根据player的位置，显示在屏幕下方|上方
    /// </summary>
    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewPortPosition = Player.Instance.GetPlayerViewPortPosition();

        if (playerViewPortPosition.y > 0.3f  && !isInventoryBarPositionBottom)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);


            isInventoryBarPositionBottom = true;
        } else if (playerViewPortPosition.y <= 0.3f && isInventoryBarPositionBottom)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 01);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);


            isInventoryBarPositionBottom = false;
        }

    }

    public void ClearHighlightInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            for (int i = 0; i < inventorySlots.Length; i ++)
            {
                if (inventorySlots[i].isSelected)
                {
                    inventorySlots[i].isSelected = false;
                    inventorySlots[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f);

                    InventoryManager.Instance.ClearSelectedVentoryItem(InventoryLocation.player);

                }
            }
        }
    }


    public void SetHighlightInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            for (int i = 0; i < inventorySlots.Length; i ++)
            {
                SetHighlightInventorySlots(i);
            }
        }
    }


    public void SetHighlightInventorySlots(int potision)
    {
        if (inventorySlots.Length > 0  && inventorySlots[potision].itemDetails != null)
        {
 
                if ( inventorySlots[potision].isSelected)
                {
                    inventorySlots[potision].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);


                InventoryManager.Instance.SetSelectedVentoryItem(InventoryLocation.player, inventorySlots[potision].itemDetails.itemCode);

                }
        }
    }

    public void DestroyCurrentDraggedItems()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {

            if (inventorySlots[i].draggedItem != null)
            {
                Destroy(inventorySlots[i].draggedItem);
            }
        
          }
    }

    public void ClearSelectedItems() {
        for (int i = 0; i < inventorySlots.Length; i ++)
        {
            inventorySlots[i].ClearSelectedItem();
        }

        ClearHighlightInventorySlots();
            
    }
}
