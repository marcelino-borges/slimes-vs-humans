using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Slime : MonoBehaviour, IDamageable
{
    [Space(10f)]
    [Header("SLIMEBASE ATTRIBUTES")]
    [SerializeField] 
    protected int _health;
    [SerializeField] 
    protected int _maxHealth;
    [SerializeField] 
    protected int _damage;
    [SerializeField] 
    protected float _lifeSpan = 5f;
    [SerializeField]
    [Tooltip("Not applicable to Slime Bomb")]
    protected float _launchForce = 100f;
    [Tooltip("Cooldown to forbid a bunch of collision detection in a single hit")]
    [SerializeField] 
    protected float _detectCollisionCooldown = .5f;
    [SerializeField]
    protected float _decayRadius = 2f;
    [SerializeField]
    protected float _cloneCooldown = .15f;
    [Tooltip("Leave empty if you don't want the slime to decay when dying")]
    [SerializeField] 
    protected SlimeType _slimeDecayType;
    protected static int _currentClonesCount;
    [Tooltip("Amount of times the slime can clone itself when Clone method is called")]
    [SerializeField] 
    protected int _maxCloneCount = 0;
    [SerializeField] 
    protected int _maxCollisionReflections = 3;
    [Tooltip("Amount of times the slime can reflect it's movement when colliding with an allowed-to-reflect object")]
    [SerializeField] 
    protected int _collisionReflectionsCount = 0;
    [SerializeField] 
    protected GameObject _explosionParticlesPrefab;
    [SerializeField]
    protected GameObject _collisionParticlesPrefab;
    [Tooltip("Whether the slime will be pooled when Die method is called")]
    [SerializeField]
    protected bool _poolObjectOnDeath = false;    
    protected bool _canDetectCollision = true;
    protected bool _canCloneItself = true;
    protected bool _isDead = false;
    protected Vector3 velocity;
    protected Rigidbody _rb;

    public int Damage { get => _damage; }
    public SlimeType SlimeDecayToSpawn { get => _slimeDecayType; }

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

    public virtual void Launch(Vector3 direction, Vector3 targetPosition, float force = 50f)
    {
#if UNITY_EDITOR
        //Debug only
        destinyPoint = transform.position + direction;
        originPoint = transform.position;
#endif
        SetVelocity(direction * force);
    }

    protected virtual void SetVelocity(Vector3 velocity)
    {
        _rb.velocity = velocity;
    }

    private void OnEnable()
    {
        _health = _maxHealth;
    }

    private void OnDestroy()
    {
        //Security call (see slime bomb calling Invoke)
        //CancelInvoke();
    }

    public virtual void Die()
    {
        if (_isDead) return;

        _isDead = true;

        _health = 0;
        
        if (_slimeDecayType != SlimeType.NONE)
            Decay();

        PlayExplosionParticles();

        if (!_poolObjectOnDeath)
            Destroy(gameObject);
        else
            Disable();
    }

    public void Decay()
    {
        //Vector3 position = new Vector3(
        //    transform.position.x + Random.Range(-_decayRadius, _decayRadius),
        //    1.72f,
        //    transform.position.z + Random.Range(-_decayRadius, _decayRadius)
        //);

        GameObject[] slimes = new GameObject[_maxCloneCount];

        for (int i = 1; i <= _maxCloneCount; i++)
        {
            ObjectPooler.instance.Spawn(_slimeDecayType, transform.position, Quaternion.identity);
        }

        foreach (GameObject slime in slimes)
        {
            Rigidbody body = slime.GetComponent<Rigidbody>();
            body.useGravity = true;
            body.constraints = RigidbodyConstraints.None;
            body.AddExplosionForce(1f, transform.position, 4f, 1f, ForceMode.Impulse);
        }
        
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public virtual void CloneItSelf()
    {
        StartCoroutine(CloneItselfCo());
    }

    protected virtual IEnumerator CloneItselfCo()
    {
        if (_canCloneItself)
        {
            _canCloneItself = false;

            for (int i = 1; i <= _maxCloneCount - 1; i++)
            {
                Vector3 position = new Vector3(
                    Random.Range(-_decayRadius, _decayRadius),
                    Random.Range(-_decayRadius, _decayRadius)
                );
                ObjectPooler.instance.Spawn(_slimeDecayType, position, Quaternion.identity);
            }
        }
        yield return new WaitForSeconds(_cloneCooldown);
        _canCloneItself = true;
    }

    protected void CountDetectCollisionCooldown()
    {
        StartCoroutine(CountDetectCollisionCooldownCo());
    }

    protected IEnumerator CountDetectCollisionCooldownCo()
    {
        _canDetectCollision = false;
        yield return new WaitForSeconds(_detectCollisionCooldown);
        _canDetectCollision = true;
    }

    protected void PlayExplosionParticles()
    {
        try
        {
            PlayParticles(_explosionParticlesPrefab);
        } 
        catch (Exception e)
        {
            Debug.LogWarning("Erro no PlayExplosionParticles() do classe Slime.cs: " + e.Message);
        }
    }

    protected void PlayCollisionParticles()
    {
        try
        {
            PlayParticles(_collisionParticlesPrefab);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Erro no PlayCollisionParticles() do classe Slime.cs: " + e.Message);
        }
    }

    protected void PlayParticles(GameObject collisionParticlesPrefab)
    {
        try
        {
            GameObject instantiated = Instantiate(collisionParticlesPrefab, transform.position, Quaternion.identity);
            instantiated.GetComponent<ParticleSystem>().Play();
        }
        catch (Exception e)
        {            
            throw e;
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if(collision != null)
        {
            if(collision.gameObject.CompareTag("Slime"))
            {
                PlayExplosionParticles();
                PlayCollisionParticles();
                CloneItSelf();
            }
        }
    }

#if UNITY_EDITOR
    protected Vector3 destinyPoint = Vector3.zero;
    protected Vector3 originPoint = Vector3.zero;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(originPoint, destinyPoint);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(_decayRadius, 0, _decayRadius));        
    }
#endif

}

