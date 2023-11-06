using System.Collections.Generic;

[System.Serializable]
public class GameSave
{
    public Dictionary<string, GameObjectSave> gameSaveData;

    public GameSave()
    {
        gameSaveData = new Dictionary<string, GameObjectSave>();
    }
}
