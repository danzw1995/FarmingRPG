using System.Collections.Generic;
[System.Serializable]
public class SceneSave 
{
    public Dictionary<string, bool> boolDictionary;
    public Dictionary<string, int> intDictionary;
    public Dictionary<string, string> stringDictionary;
    public Dictionary<string, Vector3Serilizable> vector3Dictionary;
    public Dictionary<string, int[]> intArrayDictionary;
    public List<SceneItem> sceneItemList;

    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;

    public List<InventoryItem>[] inventoryItemLists;
}
