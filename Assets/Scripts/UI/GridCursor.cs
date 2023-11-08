using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;

    // 光标有效时的sprite
    [SerializeField] private  Sprite rightCursorSprite = null;

    // 光标无效时的sprite
    [SerializeField] private Sprite wrongCursorSprite = null;

    // 作物明细
    [SerializeField] private SO_CropDetailList sO_CropDetailList = null;


    private bool _cursorPositionIsValid = false;
    
    // 光标是否有效
    public bool cursorPositionIsValid {  get { return _cursorPositionIsValid; } set { _cursorPositionIsValid = value; } }

    private int _itemUseGridRadius = 0;

    // 选中物品的使用半径
    public int itemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }


    private ItemType _selectItemType;

    // 选中物品的itemCode
    public ItemType selectedItemType { get => _selectItemType; set => _selectItemType = value; }

    private bool _cursorIsEnabled = false;

    // 光标是否启用
    public bool cursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }


    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterLoad;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterLoad;

    }

    private void AfterLoad()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if (grid != null)
        {
            Vector3Int gridPosition = GetGridPositionForCusor();

            Vector3Int playerPosition = GetGridPositionForPlayer();

            SetCursorValidity(gridPosition, playerPosition);

            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }
        return Vector3Int.zero;
    }

    /// <summary>
    /// 设置光标状态
    /// </summary>
    /// <param name="cursorGridPosition">光标位置</param>
    /// <param name="playerPosition">player位置</param>
    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerPosition)
    {
        SetCursorToValid();


        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);
        // 没有选中物品时，光标无效
        if (itemDetails == null)
        {
            SetCursorToInValid();
            return;
        }

        // 如果光标与player的距离大于工具的使用半径，光标无效
        if (Mathf.Abs(cursorGridPosition.x - playerPosition.x) > itemUseGridRadius || Mathf.Abs(cursorGridPosition.y - playerPosition.y) > itemUseGridRadius)
        {
            SetCursorToInValid();
            return;
        }

        // 获取光标位置的gridPropertyDetails
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (gridPropertyDetails != null)
        {
            switch (itemDetails.itemType)
            {
                // 种子、商品判断是否能够丢弃
                case ItemType.Seed:
                    if (!IsCursorValidSeed(gridPropertyDetails))
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;
                case ItemType.Commodity:
                    if (!IsCursorValidCommodity(gridPropertyDetails))
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;
                // 工具判断该位置是否能够使用
                case ItemType.Watering_tool:
                case ItemType.Hoeing_tool:
                case ItemType.Reaping_tool:
                case ItemType.Breaking_tool:
                case ItemType.Collecting_tool:
                case ItemType.Chopping_tool:
                    if (!IsCursorValidHoeingTool(gridPropertyDetails, itemDetails))
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;
                case ItemType.none:
                    break;
                case ItemType.count:
                    break;
                default:
                    SetCursorToInValid();
                    break;
            }
        } else
        {
            SetCursorToInValid();
            return;
        }
    }

    public void SetCursorToValid()
    {
        cursorImage.sprite = rightCursorSprite;
        cursorPositionIsValid = true;
    }

    public void SetCursorToInValid ()
    {
        cursorImage.sprite = wrongCursorSprite;
        cursorPositionIsValid = false;
    }

    public void DisableCursor()
    {
        cursorImage.color = Color.clear;
        cursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        cursorIsEnabled = true;
    }

    public bool IsCursorValidSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    public bool IsCursorValidCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    /// <summary>
    /// 判断选中的工具在鼠标位置是否能够使用
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <param name="itemDetails"></param>
    /// <returns></returns>
    public bool IsCursorValidHoeingTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                // 判断指定位置的grid是否能够锄地且没有锄地过
                if (gridPropertyDetails.isDiaggble == true && gridPropertyDetails.daysSinceDug == -1)
                {
                    Vector3 cursorWorldPosition = GetWorldPositionForCursor();
                    Vector3 newCursorWorldPosition = new Vector3(cursorWorldPosition.x + 0.5f, cursorWorldPosition.y + 0.5f, 0f);

                    List<Item> itemList = new List<Item>();

                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, newCursorWorldPosition, Settings.cursorSize, 0f);

                    bool foundReapable = false;
                    
                    // 判断itemList中是否有风景，有则光标无效
                    foreach(Item item in itemList)
                    {
                        if (InventoryManager.Instance.GetItemDetails(item.itemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }
                    }

                    return !foundReapable;
                } else
                {
                    return false;
                }
            case ItemType.Watering_tool:
                // 判断指定位置的grid是否没有浇水且进行了锄地（只有锄地之后才能浇水）
                if (gridPropertyDetails.daysSinceWatered == -1 && gridPropertyDetails.daysSinceDug > -1)
                {
                    return true;
                }
                return false;
            case ItemType.Chopping_tool:
            case ItemType.Collecting_tool:
            case ItemType.Breaking_tool:
                if (gridPropertyDetails.seedItemCode != -1)
                {
                    CropDetail cropDetail = sO_CropDetailList.GetCropDetail(gridPropertyDetails.seedItemCode);
                    if (cropDetail != null)
                    {
                        // 判断作物是否处于生长的最后一个阶段且能被收获
                        if (gridPropertyDetails.growthDays >= cropDetail.growthDays[cropDetail.growthDays.Length - 1] && cropDetail.CanUseToolToHarvestCrop(itemDetails.itemCode))
                        {
                            return true;
                        }
                    }
                }
               
                return false;
            default:
                return false;
        }

    }

    public Vector3 GetWorldPositionForCursor()
    {
        return mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

    }

    public Vector3Int GetGridPositionForCusor()
    {
        Vector3 worldPosition = GetWorldPositionForCursor();

        return grid.WorldToCell(worldPosition);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);

        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);

        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }
}
