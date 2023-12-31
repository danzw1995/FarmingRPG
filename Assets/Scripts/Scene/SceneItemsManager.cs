﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneItemsManager : SingletonMonoBehaviour<SceneItemsManager>, ISaveable
{

    private Transform parentTransform;

    [SerializeField] private GameObject itemPrefab = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID;   } set { _iSaveableUniqueID = value ; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave gameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    private void AfterSceneLoad()
    {
        parentTransform = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGuid>().GUID;
        gameObjectSave = new GameObjectSave();
    }

    private void DestroySceneItems()
    {
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

        for (int i = 0; i < itemsInScene.Length; i ++)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableList.Add(this);

    }


    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableList.Remove(this);
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameSaveData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave)) {
            this.gameObjectSave = gameObjectSave;
            ISaveableReStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public GameObjectSave ISaveableSave()
    {
        ISaveableStoreScene(SceneManager.GetActiveScene().name);


        return gameObjectSave;
    }

    /// <summary>
    /// 根据itemPosition, 实例化item
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="itemPosition"></param>
    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentTransform);
        Item item = itemGameObject.GetComponent<Item>();
        item.itemCode = itemCode;
        item.Init(itemCode);

    }

    /// <summary>
    /// 实例化场景的items
    /// </summary>
    /// <param name="sceneItems"></param>
    public void InstantiateSceneItems(List<SceneItem> sceneItems) 
    {
       foreach(SceneItem  sceneItem in sceneItems)
        {
            GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentTransform);

            Item item = itemGameObject.GetComponent<Item>();
            item.itemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }
    /// <summary>
    ///  恢复场景item
    /// </summary>
    /// <param name="sceneName"></param>
    public void ISaveableReStoreScene(string sceneName)
    {
        if (gameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.sceneItemList != null)
            {
                DestroySceneItems();

                InstantiateSceneItems(sceneSave.sceneItemList);
            }
        }
    }
   
    /// <summary>
    /// 保存场景中的Item
    /// </summary>
    /// <param name="sceneName"></param>
    public void ISaveableStoreScene(string sceneName)
    {
        gameObjectSave.sceneData.Remove(sceneName);
       Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

        List<SceneItem> sceneItems = new List<SceneItem>();

        for (int i = 0; i < itemsInScene.Length; i ++)
        {

            Item item = itemsInScene[i];
            sceneItems.Add(new SceneItem
            {
                itemCode = item.itemCode,
                itemName = item.name,
                position = new Vector3Serilizable(item.transform.position.x, item.transform.position.y, item.transform.position.z)
            });
        }
        SceneSave sceneSave = new SceneSave();
        sceneSave.sceneItemList = sceneItems;

        gameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
}
