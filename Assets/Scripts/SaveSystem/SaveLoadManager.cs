using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonoBehaviour<SaveLoadManager>
{
    public GameSave gameSave;
    public List<ISaveable> iSaveableList;
    
    protected  override void Awake()
    {
        base.Awake();

        iSaveableList = new List<ISaveable>();
    }

    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            gameSave = new GameSave();

            FileStream fileStream = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);

            gameSave = (GameSave)bf.Deserialize(fileStream);

            for (int i = iSaveableList.Count - 1; i >= 0; i --)
            {
                if (gameSave.gameSaveData.ContainsKey(iSaveableList[i].ISaveableUniqueID))
                {
                    iSaveableList[i].ISaveableLoad(gameSave);
                } else
                {
                    Component component = (Component)iSaveableList[i];
                    Destroy(component.gameObject);
                }
            }

            fileStream.Close();
        }
        UIManager.Instance.DisablePauseMenu();
    }

    public void SaveDataToFile()
    {
        gameSave = new GameSave();

        foreach(ISaveable saveableObject in iSaveableList)
        {
            gameSave.gameSaveData.Add(saveableObject.ISaveableUniqueID, saveableObject.ISaveableSave());
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        FileStream fileStream = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);

        binaryFormatter.Serialize(fileStream, gameSave);

        fileStream.Close();

        UIManager.Instance.DisablePauseMenu();
    }

    public void StoreCurrentSceneData()
    {
        foreach(ISaveable iSaveableObject in iSaveableList)
        {
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ReStoreCurrentSceneData()
    {
        foreach(ISaveable iSaveableObject in iSaveableList)
        {
            iSaveableObject.ISaveableReStoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
