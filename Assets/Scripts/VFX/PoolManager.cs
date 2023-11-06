using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMonoBehaviour<PoolManager>
{
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
    [SerializeField] private Pool[] pools = null;
    [SerializeField] private Transform poolTransform = null;

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
    }

    private void Start()
    {
       for (int i = 0; i < pools.Length; i ++)
        {
            CreatePool(pools[i].prefab, pools[i].poolSize);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;

        GameObject parentGameObject = new GameObject(prefabName + "anchor");

        parentGameObject.transform.SetParent(poolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {

            Queue<GameObject> gameObjects = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i ++)
            {
                GameObject newGameObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newGameObject.SetActive(false);
                gameObjects.Enqueue(newGameObject);

            }

            poolDictionary.Add(poolKey, gameObjects);

        }
    }

    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();
        if (poolDictionary.ContainsKey(poolKey))
        {
            GameObject objectToReuse = GetObjectFromPool(poolKey);

            ResetObjecet(position, rotation, objectToReuse, prefab);

            return objectToReuse;
        } else
        {
            Debug.Log("No object pool for" + prefab);
            return null;
        }
    }

    private GameObject GetObjectFromPool(int poolKey)
    {
        GameObject objectToReuse = poolDictionary[poolKey].Dequeue();

        poolDictionary[poolKey].Enqueue(objectToReuse);

        if (objectToReuse.activeSelf == true)
        {
            objectToReuse.SetActive(false);
        }

        return objectToReuse;

    }

    private static void ResetObjecet(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;

        objectToReuse.transform.localScale = prefab.transform.localScale;
    }

}
