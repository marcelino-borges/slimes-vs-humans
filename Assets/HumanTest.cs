using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanTest : MonoBehaviour, IDamageable
{
    public int health;
    public void ApplyDamage(int dmg)
    {
        if(health > 0)
        {
            health -= dmg;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
