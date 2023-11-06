using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropInstantiator : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private int daysSinceDug = -1;
    [SerializeField] private int daysSinceWatered = -1;
    [itemCodeDescription]
    [SerializeField] private int seedItemCode = 0;
    [SerializeField] private int growsthDays = 0;

    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateCropPrefab;
    }

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateCropPrefab;

    }

    private void InstantiateCropPrefab()
    {
        grid = GameObject.FindObjectOfType<Grid>();

        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        SetCropGridProperties(cropGridPosition);

        Destroy(gameObject);
    }

    private void SetCropGridProperties(Vector3Int cropGridPositon)
    {
        if (seedItemCode > 0)
        {
            GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPositon.x, cropGridPositon.y);
            if (gridPropertyDetails == null)
            {
                gridPropertyDetails = new GridPropertyDetails();
                gridPropertyDetails.gridX = cropGridPositon.x;
                gridPropertyDetails.gridY = cropGridPositon.y;
            }

            gridPropertyDetails.daysSinceDug = daysSinceDug;
            gridPropertyDetails.daysSinceWatered = daysSinceWatered;
            gridPropertyDetails.seedItemCode = seedItemCode;
            gridPropertyDetails.growthDays = growsthDays;

            GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        }
    }
}
