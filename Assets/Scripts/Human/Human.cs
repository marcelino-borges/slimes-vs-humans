using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class Human : MonoBehaviour, IPoolableObject
{
    #region Private Attributes
    private readonly string[] idleAnimations = new string[]
    {
        "Idle",
        "Idle_Breathing",
        "Idle_Dizzy",
        "Idle_Dwarf",
        "Idle_Happy",
        "Idle_ListeningToMusic",
        "Idle_LookAround",
        "Idle_Offensive",
        "Idle_Orc",
        "Idle_Orc2",
        "Idle_Sad",
    };
    #endregion

    #region Protected Attributes
    protected AudioSource _audioSource;
    protected bool _isInfected;
    [SerializeField]
    protected AudioClip[] _screamSfx;
    [SerializeField]
    protected Animator _animator;
    [SerializeField]
    protected bool _canBeInfected = true;
    [SerializeField]
    private bool countToTotalHumansInLevel = true;
    [SerializeField]
    protected SplatEffectHumans _splatEffect;
    #endregion

    #region Public Attributes
    public static bool canScream = true;
    public Vector3 currentDestination;
    public bool isFromPool = false;
    public Rigidbody rb;
    #endregion

    #region Public Props
    public bool IsInfected { get => _isInfected; set => _isInfected = value; }
    public bool IsFromPool { get => isFromPool; set => isFromPool = value; }
    public bool CanBeInfected { get => _canBeInfected; set => _canBeInfected = value; }
    protected bool CountToTotalHumansInLevel { get => countToTotalHumansInLevel; set => countToTotalHumansInLevel = value; }
    #endregion

    protected void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _splatEffect = GetComponent<SplatEffectHumans>();
    }

    protected void Start()
    {
        if(CountToTotalHumansInLevel)
            LevelManager.instance.IncrementHumanTotal();

        _audioSource.volume = SoundManager.instance.CurrentVolume;

        if (_animator == null)
            throw new UnassignedReferenceException("Referencie o animator dentro do game object do humano (dentro do obj da mesh?).");

        SetAnimationByName(GetIdleAnimation());
    }

    protected virtual void Update()
    {
    }

    public void OnSpawnedFromPool()
    {
        //throw new System.NotImplementedException();
    }

    public void SetIsFromPool(bool value)
    {
        isFromPool = value;
    }

    private string GetIdleAnimation()
    {
        return Utils.GetRandomArrayElement(idleAnimations);
    }

    public virtual void Infect(Slime slime)
    {
        if (IsInfected || !_canBeInfected) return;

        PlayScreamSfx();
        SetPainted(slime);
        SpawnGroupOfHumans(slime);

        _isInfected = true;
        
        LevelManager.instance.IncrementHumansInfected();
    }

    private void SpawnGroupOfHumans(Slime slime)
    {
        HumanGroup humansSpawned = ObjectPooler.instance.Spawn("human-trio", transform.position, transform.rotation).GetComponent<HumanGroup>();
        if (humansSpawned != null)
        {
            humansSpawned.SetIsFromPool(true);
            humansSpawned.gameObject.transform.SetParent(transform.parent.parent);
            humansSpawned.InfectAll(slime);
        }
    }

    public virtual void SetPainted(Slime slime)
    {
        _splatEffect.CreateSplatEffect(slime);
    }

    private void PlayScreamSfx()
    {
        if (Utils.IsArrayValid(_screamSfx) && canScream)
        {
            _audioSource.PlayOneShot(Utils.GetRandomArrayElement(_screamSfx));
            StartCoroutine(CountScreamCooldown());
        }
    }

    private IEnumerator CountScreamCooldown(float time = 3f)
    {
        canScream = false;
        yield return new WaitForSeconds(time); 
        canScream = true;
    }

    private void SetAnimationByName(string name)
    {
        _animator.Play(name);
    }

    public virtual void Die()
    {
        if (!isFromPool)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if(isFromPool)
            transform.SetParent(null);
    }
}
