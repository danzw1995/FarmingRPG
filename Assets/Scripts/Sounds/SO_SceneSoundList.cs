using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_SceneSoundList", menuName = "Scriptable Objects/Sounds/Scene Sound List")]
public class SO_SceneSoundList : ScriptableObject
{
    [SerializeField]
    public List<SceneSoundItem> sceneSoundDetails;
}
