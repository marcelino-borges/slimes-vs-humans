using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanGroup : MonoBehaviour
{
    public List<Human> humans;

    private void Awake()
    {
        foreach(Transform child in transform)
        {
            Human human = child.GetComponent<Human>();

            if (human != null)
                humans.Add(human);
        }
    }

    public void InfectAll()
    {
        foreach (Human human in humans)
        {
            human.Infect();
        }
    }
}
