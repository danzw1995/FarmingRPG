[System.Serializable]
public class SceneItem
{
    public int itemCode;
    public Vector3Serilizable position;
    public string itemName;

    public SceneItem ()
    {
        position = new Vector3Serilizable();
    }
}