using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBuildingSpawner : MonoBehaviour
{
    public GameObject buildingPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnBuilding();
        }
    }

    void SpawnBuilding()
    {
        if(buildingPrefab != null)
        {
            Instantiate(buildingPrefab, new Vector3(Random.Range(-45, 45), transform.position.y, Random.Range(-45, 45)), Quaternion.identity);
        }
    }
}
