using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolModel
{
    //Ensure tag is always in lower case
    public string tag;
    public GameObject objectToPool;
    public int amountToPool;
}

public class ObjectPooler : MonoBehaviour
{
    #region Public Attributes
    public static ObjectPooler instance;
    public List<PoolModel> objectsToPool;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public Transform defaultParent;
    #endregion

    #region Monobehavior Methods

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializePoolOfObjects();
        defaultParent = TerrainRotation.instance.gameObject.transform;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Spawn(SlimeType.COLLECTOR.ToString(), transform.position, Quaternion.identity);
        }
    }
    #endregion 

    #region Private Custom Methods

    private void InitializePoolOfObjects()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (PoolModel item in objectsToPool)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = Instantiate(item.objectToPool);
                obj.SetActive(false);
                obj.transform.SetParent(defaultParent);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(item.tag, objectPool);
        }
    }

    private GameObject GetObjectFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!poolDictionary.ContainsKey(tag.ToLower()))
        {
            //Debug.LogError("Pool de slimes com tag " + tag.ToUpper() + " não foi localizado!");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag.ToLower()].Dequeue();

        if (objectToSpawn == null)
        {
            //Debug.LogError("Não há um objeto válido na pool de slimes com tag " + tag.ToLower());
            return null;
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPoolableObject poolableObject = objectToSpawn.GetComponent<IPoolableObject>();

        if (poolableObject != null)
        {
            poolableObject.OnSpawnedFromPool();
            poolableObject.SetIsFromPool(true);
        }

        poolDictionary[tag.ToLower()].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    #endregion

    #region Public Methods

    public GameObject Spawn(SlimeType slimeType, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return GetObjectFromPool(slimeType.ToString().ToLower(), position, rotation, parent);
    }

    public GameObject Spawn(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return GetObjectFromPool(tag, position, rotation, parent);
    }

    public GameObject GetOriginalPrefabFromPool(string tag)
    {
        GameObject prefab = null;

        foreach (PoolModel pool in objectsToPool)
        {
            if (pool.tag.Equals(tag.ToLower()))
            {
                prefab = pool.objectToPool;
                break;
            }
        }
        return prefab;
    }
    #endregion
}
