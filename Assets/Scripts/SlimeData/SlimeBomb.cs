using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBomb : SlimeBase
{
    [Space(10f)]
    [Header("Expecific Attributes")]
    [SerializeField] private string _details;
    [SerializeField] private GameObject spawnObj;


    public void OnCollisionEnter(Collision collision)
    {
        IDamageable damagable = collision.gameObject.GetComponent<IDamageable>();
        if (damagable != null)
        {
            damagable.ApplyDamage(HitDamage);
        }
    }
    private void OnMouseDown()
    {
        Vector3 target = Vector3.forward;
        Launch(target);
    }

}
