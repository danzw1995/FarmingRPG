using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    [Tooltip("This shoud be populated from child gameobject showing harvest effect spawm point")]
    [SerializeField] private Transform harvestActionEffectTransform = null;

    [Tooltip("This shoud be populated from child gameobject")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer = null;
    [HideInInspector] public Vector2Int cropGridPosition;

    // 收获次数
    private int harvestActionCount = 0;

    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolLeft, bool isToolRight, bool isToolUp, bool isToolDown)
    {
        // 作物明细
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
        if (gridPropertyDetails == null)
        {
            return;
        }

        // 种子明细
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);

        if (seedItemDetails == null)
        {
            return;
        }

        // 产品明细
        CropDetail cropDetail = GridPropertiesManager.Instance.GetCropDetail(gridPropertyDetails.seedItemCode);

        if (cropDetail == null)
        {
            return;
        }
        

        // 设置使用工具动画面向
        Animator animator = GetComponentInChildren<Animator>();
        if (isToolLeft || isToolDown)
        {
            animator.SetTrigger("usetoolleft");
        } else if (isToolRight || isToolUp)
        {
            animator.SetTrigger("usetoolright");
        }

        // 收获的粒子效果
        if (cropDetail.isHarvestedActionEffect)
        {
            EventHandler.CallHarvestActionEffectEvent(harvestActionEffectTransform.position, cropDetail.harvestActionEffect);
        }

        // 获取收获次数
        int requiredHarvestActions = cropDetail.RequiredHarvestActionForTool(equippedItemDetails.itemCode);

        if (requiredHarvestActions == -1)
        {
            return;
        }

        harvestActionCount += 1;

        // 收获次数大于等于作物的收获次数时收获作物
        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(cropDetail, gridPropertyDetails, animator, isToolRight, isToolUp);
        }
    }

    private void HarvestCrop(CropDetail cropDetail, GridPropertyDetails gridPropertyDetails, Animator animator, bool isToolRight, bool isToolUp)
    {

        if (cropDetail.isHarvestedAnimation && animator != null)
        {
            if (cropDetail.harvestSprite != null)
            {
                cropHarvestedSpriteRenderer.sprite = cropDetail.harvestSprite;
            }
            // 设置收获动画参数

            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("harvestright");

            }
            else
            {
                animator.SetTrigger("harvestleft");

            }

        }

        if (cropDetail.harvestSound != SoundName.none)
        {
            // 播放收获音频
            AudioManager.Instance.PlaySound(cropDetail.harvestSound);
        }

        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        if (cropDetail.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        if (cropDetail.disableCropBeforeHarvestedAnimation)
        {
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
           foreach(Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        if (cropDetail.isHarvestedAnimation && animator != null)
        {
            // 收获动画
            StartCoroutine(ProcessHarvestActionAfterAnimation(cropDetail, gridPropertyDetails, animator));
        } else
        {
            HarvestAction(cropDetail, gridPropertyDetails);

        }


    }

    private IEnumerator ProcessHarvestActionAfterAnimation(CropDetail cropDetail, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        while(!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }
        HarvestAction(cropDetail, gridPropertyDetails);

    }

    private void HarvestAction(CropDetail cropDetail, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItem(cropDetail);

        // 是否具有下阶段的作物
        if (cropDetail.harvestTransformItemCode > 0)
        {
            CreateHarvestedTransformCrop(cropDetail, gridPropertyDetails);
        }

        Destroy(gameObject);

    }

    private void SpawnHarvestedItem(CropDetail cropDetail)
    {
        for (int i = 0; i < cropDetail.cropProducedItemCodes.Length; i ++)
        {
            int cropToProduce;

            // 设置收获的数量
            if (cropDetail.cropProducedMaxQuantity[i] <= cropDetail.cropProducedMinQuantity[i])
            {
                cropToProduce = cropDetail.cropProducedMinQuantity[i];
            } else
            {
                cropToProduce = Random.Range(cropDetail.cropProducedMinQuantity[i], cropDetail.cropProducedMaxQuantity[i]);
            }

            for (int j = 0; j < cropToProduce; j ++)
            {
                Vector3 spawnPosition;
                
                // 收获有两种情况
                if (cropDetail.spawnProducedAtPlayerPosition)
                {
                    // 1. 直接产物添加到背包
                    InventoryManager.Instance.AddItem(InventoryLocation.player, cropDetail.cropProducedItemCodes[i]);
                }
                else
                {
                    // 2.在场景中添加产物

                    // 设置产物的坐标
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f), 0f);

                    // 添加到场景中
                    SceneItemsManager.Instance.InstantiateSceneItem(cropDetail.cropProducedItemCodes[i], spawnPosition);
                }
            }
        }
    }

    /// <summary>
    /// 创建下一阶段的作物
    /// </summary>
    /// <param name="cropDetail"></param>
    /// <param name="gridPropertyDetails"></param>
    private void CreateHarvestedTransformCrop(CropDetail cropDetail, GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.seedItemCode = cropDetail.harvestTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        // 更新作物明细
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // 重新种植
        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

    }
}
