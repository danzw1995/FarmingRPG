using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : SingletonMonoBehaviour<VFXManager>
{
    private WaitForSeconds twoSeconds;
    [SerializeField] private GameObject reapingPrefab = null;
    [SerializeField] private GameObject decidousLeavesFallingPrefab = null;
    [SerializeField] private GameObject PineConesFallingPrefab = null;
    [SerializeField] private GameObject breakingStonePrefab = null;
    [SerializeField] private GameObject choppingTreeTrunkPrefab = null;

    protected override void Awake()
    {
        base.Awake();

        twoSeconds = new WaitForSeconds(2f);
    }

    private void OnDisable()
    {
        EventHandler.HarvestActionEffectEvent -= DisplayHarvestActionEffect;

    }

    private void OnEnable()
    {
        EventHandler.HarvestActionEffectEvent += DisplayHarvestActionEffect;
    }

    private void DisplayHarvestActionEffect(Vector3 position, HarvestActionEffect harvestActionEffect)
    {
        switch (harvestActionEffect)
        {
            case HarvestActionEffect.choopingTreeTrunk:
                GameObject choppingTreeTrunk = PoolManager.Instance.ReuseObject(choppingTreeTrunkPrefab, position, Quaternion.identity);
                choppingTreeTrunk.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(choppingTreeTrunk, twoSeconds));
                break;
            case HarvestActionEffect.breakingStone:
                GameObject breakingStone = PoolManager.Instance.ReuseObject(breakingStonePrefab, position, Quaternion.identity);
                breakingStone.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(breakingStone, twoSeconds));
                break;
            case HarvestActionEffect.deciduousLeavesFalling:
                GameObject decidousLeavesFalling = PoolManager.Instance.ReuseObject(decidousLeavesFallingPrefab, position, Quaternion.identity);
                decidousLeavesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(decidousLeavesFalling, twoSeconds));
                break;
            case HarvestActionEffect.pineconesFalling:
                GameObject pineconesFalling = PoolManager.Instance.ReuseObject(PineConesFallingPrefab, position, Quaternion.identity);
                pineconesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(pineconesFalling, twoSeconds));
                break;
            case HarvestActionEffect.reaping:
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, position, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(reaping, twoSeconds));
                break;
            case HarvestActionEffect.none:
                break;
            default: 
                break;

        }
    }

    private IEnumerator DisableHarvestActionEffect(GameObject reaping, WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        reaping.SetActive(false);
    }
}
