
public interface ISaveable
{
    string ISaveableUniqueID { get; set; }

    GameObjectSave gameObjectSave { get; set; }

    void ISaveableRegister();
    void ISaveableDeregister();

    GameObjectSave ISaveableSave();

    void ISaveableLoad(GameSave gameSave);

    void ISaveableStoreScene(string sceneName);

    void ISaveableReStoreScene(string sceneName);

}
