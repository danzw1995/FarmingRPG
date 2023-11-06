using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDetailList", menuName = "Scriptable Objects/Crop/Crop Detail List")]
public class SO_CropDetailList : ScriptableObject
{
    [SerializeField]
    public List<CropDetail> cropDetails;

    public CropDetail GetCropDetail(int seedItemCode)
    {
        return cropDetails.Find(crop => crop.seedItemCode == seedItemCode);
    }
}
