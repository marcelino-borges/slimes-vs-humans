using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanGroup : MonoBehaviour, IPoolableObject
{
    public List<Human> humans;
    public bool isFromPool = false;

    protected void Awake()
    {
        foreach(Transform child in transform)
        {
            Human human = child.GetComponent<Human>();

            if (human != null && human.gameObject.activeInHierarchy)
                humans.Add(human);
        }
    }

    public void InfectAll(Slime slime)
    {
        foreach (Human human in humans)
        {
            human.SetPainted(slime);
            human.IsInfected = true;
        }
    }

    private void OnDisable()
    {
        if(isFromPool)
            transform.SetParent(null);
    }

    public void OnSpawnedFromPool()
    {
    }

    public void SetIsFromPool(bool value)
    {
        isFromPool = value;
        //throw new System.NotImplementedException();
    }
}
