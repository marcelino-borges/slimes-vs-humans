using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Slime : MonoBehaviour, IDamageable
{
    [Space(10f)]
    [Header("SLIMEBASE ATTRIBUTES")]
    [SerializeField] private int _health;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _damage;
    [SerializeField] private float _lifeSpan = 2f;
    [SerializeField] private float _groundSpeed;
    [SerializeField] private float _launchForce;
    [SerializeField] private float _launchInclinationAngle;
    [SerializeField] private GameObject _slimeDecayPrefab;
    [SerializeField] private static int _currentClonesCount;
    [SerializeField] private int _maxCloneCount = 0;
    [SerializeField] private Rigidbody _rb;

    public float GroundSpeed { get => _groundSpeed; }
    public int Damage { get => _damage; }
    public GameObject SlimeDecayToSpawn{get => _slimeDecayPrefab;}

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        Destroy(gameObject, _lifeSpan);
    }

    public virtual void ApplyDamage(int dmg)
    {
        if (_health > 0)
            _health -= dmg;
        else
            gameObject.SetActive(false);
    }

    public virtual void CloneItSelf(GameObject ObjToClone, bool canClone)
    {
        if (canClone)
        {
            for (int i = _currentClonesCount; i <= _maxCloneCount - 1; i++)
            {
                Instantiate(ObjToClone, transform.position, Quaternion.identity);
            }
        }
    }

    public void Decay()
    {
        Instantiate(_slimeDecayPrefab, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    public virtual void Launch(Vector3 direction, float force = 50f)
    {
        print("force of launch = " + force);
#if UNITY_EDITOR
        //Debug only
        destinyPoint = transform.position + direction;
        originPoint = transform.position;
#endif        
        _launchForce = force;
        _rb.velocity = direction * _launchForce;
    }

    private void OnEnable()
    {
        _health = _maxHealth;
    }

    private void OnDisable()
    {
        _health = 0;
    }

#if UNITY_EDITOR
    Vector3 destinyPoint = Vector3.zero;
    Vector3 originPoint = Vector3.zero;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(originPoint, destinyPoint);
    }
#endif

}
