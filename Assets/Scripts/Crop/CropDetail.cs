using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropDetail
{
    [itemCodeDescription]
    public int seedItemCode;
    public int[] growthDays;
    public GameObject[] growthPrefabs;
    public Sprite[] growthSprites;
    public Season[] seasons;
    public Sprite harvestSprite;

    [itemCodeDescription]
    public int harvestTransformItemCode;
    public bool hideCropBeforeHarvestedAnimation;
    public bool disableCropBeforeHarvestedAnimation;


    public bool isHarvestedAnimation;
    public bool isHarvestedActionEffect;
    public bool spawnProducedAtPlayerPosition;
    public HarvestActionEffect harvestActionEffect;
    public SoundName harvestSound;

    [itemCodeDescription]
    public int[] harvestToolItemCodes;
    public int[] requiredHarvestActions;

    [itemCodeDescription]
    public int[] cropProducedItemCodes;
    public int[] cropProducedMinQuantity;
    public int[] cropProducedMaxQuantity;
    public int daysToRegrow;


    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionForTool(toolItemCode) == -1)
        {
            return false;
        }
        return true;
    }

    public int RequiredHarvestActionForTool(int toolItemCode)
    {
        for (int i = 0; i < harvestToolItemCodes.Length; i ++)
        {
            if (harvestToolItemCodes[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }

}