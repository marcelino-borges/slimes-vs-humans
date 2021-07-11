using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Slime : MonoBehaviour, IDamageable
{
    [Space(10f)]
    [Header("SLIMEBASE ATTRIBUTES")]
    [SerializeField] protected int _health;
    [SerializeField] protected int _maxHealth;
    [SerializeField] protected int _damage;
    [SerializeField] protected float _lifeSpan = 5f;
    [SerializeField] protected float _groundSpeed;
    [SerializeField] protected float _launchInclinationAngle;
    [SerializeField] protected GameObject _slimeDecayPrefab;
    [SerializeField] protected static int _currentClonesCount;
    [SerializeField] protected int _maxCloneCount = 0;
    [SerializeField] protected Rigidbody _rb;

    protected float _launchForce = 10f;
    public float GroundSpeed { get => _groundSpeed; }
    public int Damage { get => _damage; }
    public GameObject SlimeDecayToSpawn{ get => _slimeDecayPrefab; }

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

    public virtual void Launch(Vector3 direction, Vector3 targetPosition, float force = 50f, float radianAngle = 0)
    {
#if UNITY_EDITOR
        //Debug only
        destinyPoint = transform.position + direction;
        originPoint = transform.position;
#endif
        SetVelocity(direction * force);
    }

    protected void SetVelocity(Vector3 direction)
    {
        _rb.velocity = direction;
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
    protected Vector3 destinyPoint = Vector3.zero;
    protected Vector3 originPoint = Vector3.zero;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(originPoint, destinyPoint);
    }
#endif

}
