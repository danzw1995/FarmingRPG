using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite transparentCursorSprite = null;
    [SerializeField] private GridCursor gridCursor = null;

    private bool _cursorPositionIsValid = false;
    public bool cursorPositionIsValid { get { return _cursorPositionIsValid; } set { _cursorPositionIsValid = value; } }

    private float _itemUseRadius = 0;

    public float itemUseRadius { get => _itemUseRadius; set => _itemUseRadius = value; }

    private ItemType _selectItemType;

    public ItemType selectedItemType { get => _selectItemType; set => _selectItemType = value; }

    private bool _cursorIsEnabled = false;

    public bool cursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (cursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private void DisplayCursor()
    {
        Vector3 cursorWorldPosition = GetWorldPositionForCursor();

        SetCursorValidity(cursorWorldPosition, Player.Instance.GetPlayerCentrePosition());

        cursorRectTransform.position = GetRectTransformPositionForCursor();
    }

    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        SetCursorToValid();
        if (
            cursorPosition.x > (playerPosition.x + itemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + itemUseRadius / 2f)
            ||
            cursorPosition.x > (playerPosition.x + itemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - itemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPosition.x - itemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + itemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPosition.x - itemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - itemUseRadius / 2f)
            )
        {
            Debug.Log("Invalid---1");
            SetCursorToInValid();
            return;
        }

        if (Mathf.Abs(cursorPosition.x - playerPosition.x) > itemUseRadius || Mathf.Abs(cursorPosition.y - playerPosition.y) > itemUseRadius)
        {
            Debug.Log("Invalid---2");
            SetCursorToInValid();
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails == null)
        {
            Debug.Log("Invalid---3");
            SetCursorToInValid();
            return;
        }

        switch(itemDetails.itemType)
        {
            case ItemType.Watering_tool:
            case ItemType.Hoeing_tool:
            case ItemType.Reaping_tool:
            case ItemType.Breaking_tool:
            case ItemType.Collecting_tool:
            case ItemType.Chopping_tool:
                if (!SetCursorValidityTool(cursorPosition, playerPosition, itemDetails)) {
                    Debug.Log("Invalid---4");
                    SetCursorToInValid();
                    return;
                }
                break;
            case ItemType.none:
                break;
            case ItemType.count:
                break;
            default:
                break;
        }
    }

    private bool SetCursorValidityTool (Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {

        switch (itemDetails.itemType)
        {
            case ItemType.Reaping_tool:
                return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemDetails);
            default:
                return false;
        }
    }

    private bool SetCursorValidityReapingTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        List<Item> items = new List<Item>();

        if (HelperMethods.GetComponentsAtCursorLocation(out items, cursorPosition))
        {
            foreach(Item item in items)
            {
                if (InventoryManager.Instance.GetItemDetails(item.itemCode).itemType == ItemType.Reapable_scenary)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        cursorPositionIsValid = true;

        gridCursor.DisableCursor();
    }

    public void SetCursorToInValid()
    {
        cursorImage.sprite = transparentCursorSprite;
        cursorPositionIsValid = false;

        gridCursor.EnableCursor();
    }

    public void DisableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 0f);
        cursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        cursorIsEnabled = true;
    }

    public Vector3 GetWorldPositionForCursor()
    {
        return mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));

    }

    public Vector2 GetRectTransformPositionForCursor()
    {
        Vector3 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTransform, canvas);
    }

}
