using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SlimeBomb : SlimeBase
{
    [Space(10f)]
    [Header("Expecific Attributes")]
    [SerializeField] private string _details;
    [SerializeField] private GameObject spawnObj;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Collectable")
        {
            Debug.Log(collision);
            Explode();
        }
    }

}
