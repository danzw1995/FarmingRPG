using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [itemCodeDescription]
    [SerializeField]
    private int _itemCode;

    public int itemCode { get { return _itemCode; } set { _itemCode = value; } }


    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (itemCode != 0)
        {
            Init(itemCode);
        }
    }

   public void Init(int itemCode) {

        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

        spriteRenderer.sprite = itemDetails.itemSprite;

        if (itemDetails.itemType == ItemType.Reapable_scenary)
        {
            gameObject.AddComponent<ItemNudge>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
