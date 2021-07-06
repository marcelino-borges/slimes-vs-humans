using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlimeBase : MonoBehaviour, IDamageable
{
    [Space(10f)]
    [Header("Basics Attributes")]
    [SerializeField] private int _health;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _hitDamage;
    [SerializeField] private float _speedMovementInGroud;
    [SerializeField] private float _speedToLaunch;
    [SerializeField] private GameObject _slimePrefab;
    [SerializeField] private static int _currentClonesCount;
    [SerializeField] private int _maxCloneCount = 0;

    public float Speed { get => _speedMovementInGroud; }
    public int HitDamage { get => _hitDamage; }

    public void ApplyDamage(int dmg)
    {
        if (_health > 0)
            _health -= dmg;
        else
            this.gameObject.SetActive(false);
    }

    public void CloneItSelf(GameObject ObjToClone, bool canClone)
    {
        if (canClone)
        {
            for (int i = _currentClonesCount; i <= _maxCloneCount - 1; i++)
            {
                //Instantiate(ObjToClone, transform.position, Quaternion.identity);
            }
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
        //var targetDirn = transform.forward;
        //var elevationAxis = transform.right;

        var releaseAngle = 30f;
        //var releaseSpeed = 30f;

        var releaseVector = Quaternion.AngleAxis(releaseAngle, transform.right) * direction;
        this.gameObject.GetComponent<Rigidbody>().velocity = releaseVector * _speedToLaunch;

    }
    private void OnEnable()
    {
        _health = _maxHealth;
    }
    private void OnDisable()
    {
        _health = 0;
    }

}
