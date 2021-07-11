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
    public static ObjectPooler instance;
    public List<PoolModel> objectsToPool;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializePoolOfObjects();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Spawn(SlimeType.COLLECTOR.ToString(), transform.position, Quaternion.identity);
        }
    }

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
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(item.tag, objectPool);
        }
    }

    public GameObject Spawn(SlimeType slimeType, Vector3 position, Quaternion rotation)
    {
        return GetObjectFromPool(slimeType.ToString().ToLower(), position, rotation);
    }

    public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
    {
        return GetObjectFromPool(tag, position, rotation);
    }

    private GameObject GetObjectFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag.ToLower()))
        {
            Debug.LogError("Pool de slimes com tag " + tag.ToLower() + " não foi localizado!");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag.ToLower()].Dequeue();

        if (objectToSpawn == null)
        {
            Debug.LogError("Não há um objeto válido na pool de slimes com tag " + tag.ToLower());
            return null;
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPoolableObject poolableObject = objectToSpawn.GetComponent<IPoolableObject>();

        if (poolableObject != null)
            poolableObject.OnSpawnedFromPool();

        poolDictionary[tag.ToLower()].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
