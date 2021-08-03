using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

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
    protected Material[] _originalMaterials;
    protected bool _isInfected;
    [SerializeField]
    protected bool _canBeInfected = true;
    [SerializeField]
    private bool countToTotalHumansInLevel = true;
    [SerializeField]
    protected AudioClip[] _screamSfx;
    [SerializeField]
    protected Animator _animator;
    [SerializeField]
    protected SplatEffectHumans _splatEffect;
    [SerializeField]
    protected SkinnedMeshRenderer _mesh;
    #endregion

    [Space(5), Header("the kind of slime that the human becomes"), SerializeField] // ***To slime effect
    private VisualEffect turnSlimeVFX;
    private CapsuleCollider humanColl;

    #region Public Attributes
    public static bool canScream = true;
    public Rigidbody rb;
    [ReadOnly]
    public bool isFromPool = false;
    #endregion

    #region Public Props
    public bool IsInfected { get => _isInfected; set => _isInfected = value; }
    public bool IsFromPool { get => isFromPool; set => isFromPool = value; }
    public bool CanBeInfected { get => _canBeInfected; set => _canBeInfected = value; }
    protected bool CountToTotalHumansInLevel { get => countToTotalHumansInLevel; set => countToTotalHumansInLevel = value; }
    #endregion

    #region IPoolableObject implementation
    public void OnSpawnedFromPool()
    {
        //throw new System.NotImplementedException();
    }

    public void SetIsFromPool(bool value)
    {
        isFromPool = value;
    }
    #endregion

    protected void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _splatEffect = GetComponent<SplatEffectHumans>();

        if (_mesh != null)
            _originalMaterials = _mesh.materials;

        humanColl = GetComponent<CapsuleCollider>();
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            //StartCoroutine(HumantToSlimeCR());
        }
    }

    private string GetIdleAnimation()
    {
        return Utils.GetRandomArrayElement(idleAnimations);
    }

    public virtual void Infect(Slime slime)
    {
        if (IsInfected || !_canBeInfected || !LevelManager.instance.IsGameActive()) return;

        PlayScreamSfx();
        SetPainted(slime);
        SetAnimationByName("Infect");

        _isInfected = true;
        
        LevelManager.instance.IncrementHumansInfected();
        GameManager.instance.VibrateAndShake();

        //StartCoroutine(HumantToSlimeCR()); // ***To slime effect
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

        _mesh.materials = _originalMaterials;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag(GameManager.TERRAIN_TAG))
                rb.useGravity = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag(GameManager.TERRAIN_TAG))
                rb.useGravity = true;
        }
    }

    private IEnumerator HumantToSlimeCR()
    {
        yield return new WaitForSeconds(0f);
        //humanColl.enabled = false;
        //turnSlimeVFX.Play();
        //_mesh.enabled = false;

        //yield return new WaitForSeconds(0.5f);
        //ObjectPooler.instance.Spawn(SlimeType.COLLECTOR, transform.position, Quaternion.identity);

        //yield return new WaitForSeconds(1.5f);
        //turnSlimeVFX.enabled = false;
        //Destroy(this);
    }
}
