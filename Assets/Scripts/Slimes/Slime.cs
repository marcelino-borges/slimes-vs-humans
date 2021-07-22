using MilkShake;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public abstract class Slime : MonoBehaviour, IDamageable, IPoolableObject
{
    private bool _canDecay = true;
    private Vector3 _positionOnLaunch = Vector3.zero;
    private Vector3 _targetPosition = Vector3.zero;
    private float time = 0;

    [Header("SLIMEBASE ATTRIBUTES")]
    [Space(10f)]
    [SerializeField] 
    protected int _health;
    [SerializeField] 
    protected int _maxHealth;
    [SerializeField] 
    protected int _damage;
    [SerializeField]
    protected float _lifeSpan = 9999999f;
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
    [SerializeField]
    protected float _canCloneCooldown = 1f;
    [SerializeField]
    protected float _canDecayCooldown = 1f;
    protected float _cloneSfxCooldown = .25f;
    [Tooltip("Leave empty if you don't want the slime to decay when dying")]
    [SerializeField]
    protected SlimeType _slimeDecayType = SlimeType.NONE;
    protected SlimeType _slimeCloneType = SlimeType.NONE;
    [Tooltip("Amount of times the slime can clone itself when Clone method is called")]
    [SerializeField] 
    protected int _maxCloneCount = 2;
    [SerializeField]
    protected int _currentCloneCount = 0;
    [SerializeField] 
    protected int _maxCollisionReflections = 3;
    [Tooltip("Amount of times the slime can reflect it's movement when colliding with an allowed-to-reflect object")]
    [SerializeField] 
    protected int _collisionReflectionsCount = 0;
    [SerializeField] 
    protected GameObject _explosionParticlesPrefab;
    [SerializeField]
    protected GameObject _collisionParticlesPrefab;
    protected AudioSource _audioSource;
    [SerializeField]
    protected AudioClip[] _collisionSfx;
    [SerializeField]
    protected AudioClip _deathSfx;
    [SerializeField]
    protected AudioClip _decaySfx;
    [Tooltip("Whether the slime will be pooled when Die method is called")]
    [SerializeField]
    protected bool _poolObjectOnDeath = false;
    protected bool _canDetectCollision = true;
    protected bool _isDead = false;
    protected bool _movingInTrajectory = false;
    protected Vector3 velocity;
    [SerializeField]
    protected SkinnedMeshRenderer _bodyMesh;
    [SerializeField]
    protected Material _decayMaterial;
    protected Material _originalBodyMaterial;

    public static int _currentGlobalClonesCount = 0;
    public static int _maxGlobalClonesCount = 200;

    public Rigidbody rb;
    public bool isGroundMode;
    public bool isClone = false;
    public static bool isVibrating;
    public bool isFromPool = false;
    public bool isSterile = false;
    public ShakePreset shakePreset;
    public UnityEvent OnDieEvent;

    public int Damage { get => _damage; }
    public SlimeType SlimeDecayToSpawn { get => _slimeDecayType; }
    public float DecayRadius { get => _decayRadius; set => _decayRadius = value; }

    #region IPoolableObject implementation
    public void OnSpawnedFromPool()
    {
        _health = _maxHealth;
        isClone = true;
        _poolObjectOnDeath = true;
        SetOnGroundMode();
    }

    public void SetIsFromPool(bool value)
    {
        isFromPool = value;
    }
    #endregion

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();

        if (OnDieEvent == null)
            OnDieEvent = new UnityEvent();

        _originalBodyMaterial = _bodyMesh.material;
    }

    protected virtual void Start()
    {
        //Destroy(gameObject, _lifeSpan);
        _audioSource.volume = SoundManager.instance.CurrentVolume;
    }

    protected virtual void FixedUpdate()
    {
        if (_movingInTrajectory)
        {
            time += Time.deltaTime;
            float x = time * _targetPosition.x;
            float z = time * _targetPosition.z;
            float y = Utils.GetYWhenAtZPosition(
                z,
                _launchForce,
                LaunchTrajectory.degreeAngle * Mathf.Deg2Rad,
                Mathf.Abs(Physics.gravity.y),
                _positionOnLaunch.y
            );
            Vector3 nextPosition = new Vector3(x, y, z);
            rb.MovePosition(nextPosition);
        }
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
        _positionOnLaunch = transform.position;
        _targetPosition = targetPosition;
        _launchForce = force;
        _movingInTrajectory = true;
        SetVelocity(direction * _launchForce);
#if UNITY_EDITOR
        //Debug only
        destinyPoint = transform.position + direction;
        originPoint = transform.position;
#endif
        transform.SetParent(TerrainRotation.instance.transform);
    }

    protected virtual void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public virtual void Die()
    {
        if (_isDead) return;

        _isDead = true;

        _health = 0;
        
        //if (_slimeDecayType != SlimeType.NONE)
        //    Decay();

        PlayExplosionParticles();
        PlaySfx(_deathSfx);
        ShakeCamera();
        Vibrate();
        OnDieEvent.Invoke();
        transform.SetParent(TerrainRotation.instance.gameObject.transform);

        if (!isFromPool)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    public void Decay()
    {
        StartCoroutine(DecayCo());
    }

    protected IEnumerator DecayCo()
    {
        if (_canDecay && !isGroundMode)
        {
            StartCoroutine(CountCanDecayCooldown());
            PlaySfx(_decaySfx);

            for (int i = 1; i <= _maxCloneCount; i++)
            {
                GameObject obj = ObjectPooler.instance.Spawn(_slimeDecayType, GetPositionInRadius(), Quaternion.identity);

                if (obj != null)
                {
                    Slime slime = obj.GetComponent<Slime>();

                    if (slime != null)
                    {
                        slime.SetOnGroundMode();
                        slime.isClone = true;
                    }
                }
                yield return new WaitForSeconds(_cloneCooldown);
            }
            Die();
        }
    }

    public void Disable()
    {
        if (isClone && _currentGlobalClonesCount > 0)
            _currentGlobalClonesCount--;
        gameObject.SetActive(false);
    }

    public virtual void CloneItSelf()
    {
        StartCoroutine(CloneItselfCo());
    }

    protected virtual IEnumerator CloneItselfCo()
    {
        if (!isSterile && _currentCloneCount < _maxCloneCount)
        {
            //StartCoroutine(CountCanCloneCooldown());
            _currentGlobalClonesCount++;
            _currentCloneCount++;

            for (int i = 1; i <= _maxCloneCount; i++)
            {
                if (_currentGlobalClonesCount < _maxGlobalClonesCount)
                {
                    Vector3 position = GetPositionInRadius();
                    ObjectPooler.instance.Spawn(_slimeCloneType, position, Quaternion.identity);
                    PlaySfx(_decaySfx);
                    yield return new WaitForSeconds(Random.Range(_cloneCooldown - .2f, _cloneCooldown + .2f));
                }
                else
                {
                    break;
                }
            }
            Sterilize();
            //Die();
        }
    }

    protected void Sterilize()
    {
        isSterile = true;
        SetSterileMaterial(_decayMaterial);
    }

    protected void SetSterileMaterial(Material material)
    {
        if (_bodyMesh != null && material != null)
        {
            _bodyMesh.material = material;
        }
    }

    private IEnumerator CountCanDecayCooldown()
    {
        _canDecay = false;
        yield return new WaitForSeconds(_canDecayCooldown);
        _canDecay = true;
    }

    protected Vector3 GetPositionInRadius()
    {
        return new Vector3(
            transform.position.x + Random.Range(-_decayRadius, _decayRadius),
            1.72f,
            transform.position.z + Random.Range(-_decayRadius, _decayRadius)
        );
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

    protected virtual void PlaySfx(AudioClip clip)
    {
        if(_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    protected virtual void TestCollisionAgainstBuildings(Collision collision)
    {
        if (collision.gameObject.CompareTag("Building"))
        {
            ShakeCamera();
        }
    }

    protected virtual void TestCollisionAgainstHumans(Collision collision)
    {
        if (collision.gameObject.CompareTag("Human"))
        {
            Human human = collision.gameObject.GetComponent<Human>();
            if (human != null)
            {
                human.rb.isKinematic = true;
                human.Infect(this);
            }

            Die();
        }
    }

    protected virtual void TestCollisionAgainstObstacles(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();

            if (obstacle.killSlime)
                Die();
        }
    }

    protected virtual void TestCollisionAgainstSlimes(Collision collision)
    {
        if (collision.gameObject.CompareTag("Slime"))
        {
            CloneItSelf();
            //Die();
        }
    }

    protected virtual void TestCollisionAgainstTerrain(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            transform.SetParent(TerrainRotation.instance.gameObject.transform);
        }
    }

    protected bool CanDetectCollision()
    {
        return _canDetectCollision;
    }

    protected virtual void SetOnGroundMode()
    {
        _movingInTrajectory = false;
        rb.useGravity = true;
        rb.AddForce(velocity * 50f);
        isGroundMode = true;
    }

    protected virtual void ShakeCamera()
    {
        if(shakePreset != null)
            Shaker.ShakeAll(shakePreset);
    }

    protected void Vibrate()
    {
        if (!isVibrating)
        {
            Handheld.Vibrate();
            StartCoroutine(VibrateCo());
        }
    }

    private IEnumerator VibrateCo()
    {
        isVibrating = true;
        yield return new WaitForSeconds(1f);
        isVibrating = false;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            TestCollisionAgainstTerrain(collision);

            if (CanDetectCollision())
            {
                PlayCollisionParticles();

                //TestCollisionAgainstSlimes(collision);
                SetOnGroundMode();
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        if (isClone && _currentGlobalClonesCount > 0)
            _currentGlobalClonesCount--;


    }

    private void OnDestroy()
    {
        StopAllCoroutines();

        if (isClone && _currentGlobalClonesCount > 0)
            _currentGlobalClonesCount--;

        SetSterileMaterial(_originalBodyMaterial);
    }

#if UNITY_EDITOR
    protected Vector3 destinyPoint = Vector3.zero;
    protected Vector3 originPoint = Vector3.zero;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(originPoint, destinyPoint);
        //Decay radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(_decayRadius, 0, _decayRadius));        
    }
#endif

}

