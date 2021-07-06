using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlimeBase : MonoBehaviour
{
    [Space(10f)]
    [Header("Basics Attributes")]
    [SerializeField] private int _health;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _hitDamage;
    [SerializeField] private float _damageRadius;
    [SerializeField] private float _explosionPower;
    [SerializeField] private float _upExplosionPower;
    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _trgLayerToDmg;
    [SerializeField] private GameObject _slimePrefab;
    [SerializeField] private static int _currentClonesCount;
    [SerializeField] private int _maxCloneCount = 0;

    public void ApplyDamage(int dmg)
    {
        _health -= dmg;
        if (_health <= 0)
            Debug.Log("Die");
    }
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

    public void CloneItSelf(GameObject ObjToClone, bool canClone)
    {
        if (canClone)
        {
            for (int i = _currentClonesCount; i <= _maxCloneCount - 1; i++)
            {
                Instantiate(ObjToClone, transform.position, Quaternion.identity);
            }
            Explode();
        }
    }
    public void Decay()
    {

    }
    public void DecreaseHitPoints()
    {

    }
    public void Launch(Vector3 direction)
    {

    }
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    public LayerMask TargetToDmg() => _trgLayerToDmg;

}
