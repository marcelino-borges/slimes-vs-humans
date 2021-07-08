using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeExplosion : MonoBehaviour
{
    [SerializeField] private float _damageRadius;
    [SerializeField] private float _explosionPower;
    [SerializeField] private float _upExplosionPower;
    
    public void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, _damageRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(_explosionPower, explosionPos, _damageRadius, _upExplosionPower);
            }
        }
    }
}
