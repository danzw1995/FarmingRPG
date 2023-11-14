using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropDetail
{
    [itemCodeDescription]
    // 种子code
    public int seedItemCode;
    // 各成长阶段的天数
    public int[] growthDays;
    // 各成长阶段的prefab
    public GameObject[] growthPrefabs;
    // 各成长阶段的sprite
    public Sprite[] growthSprites;
    public Season[] seasons;
    public Sprite harvestSprite;

    [itemCodeDescription]
    // 收获的阶段产物itemCode（应用于可多次收获的情况，这里树经过砍伐，收获了木头，然后变成木柱，木柱又可以再次收获）
    public int harvestTransformItemCode;

    // 在收获动画结束前隐藏产物sprite
    public bool hideCropBeforeHarvestedAnimation;

    // 在收获动画结束前隐藏产物的collider2D(防止与player发生碰撞)
    public bool disableCropBeforeHarvestedAnimation;

    // 是否有收获动画
    public bool isHarvestedAnimation;

    // 是否有收获粒子效果
    public bool isHarvestedActionEffect;

    // 是否直接将产品添加到player背包
    public bool spawnProducedAtPlayerPosition;

    // 收获粒子效果
    public HarvestActionEffect harvestActionEffect;

    // 收获音频
    public SoundName harvestSound;

    [itemCodeDescription]
    // 收获的工具itemCode数组
    public int[] harvestToolItemCodes;
    // 收获的工具对应的收获次数， 通过索引和收获的工具一一对应
    public int[] requiredHarvestActions;

    [itemCodeDescription]

    // 产物itemCode
    public int[] cropProducedItemCodes;

    // 产物最小值
    public int[] cropProducedMinQuantity;

    // 产物最大值
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

    /// <summary>
    ///   根据工具找出需要收获的次数，如果不能收获，返回-1，否则返回对应的次数
    /// </summary>
    /// <param name="toolItemCode"></param>
    /// <returns></returns>
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