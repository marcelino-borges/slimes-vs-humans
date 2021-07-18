using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
public class Human : MonoBehaviour
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

    private readonly string walkingAnimation = "Walking";
    private readonly string runningAnimation = "Running";
    private readonly string goofyRunningAnimation = "Goofy Running";
    private readonly string injuriedRunAnimation = "InjuredRun";
    private readonly string painGestureRunAnimation = "PainGesture";
    private readonly string injuredStumbleIdleAnimation = "InjuredStumbleIdle";
    private float _timerScaredAndStuck = 0f;
    private float _checkingTimeCounter = 0f;
    #endregion

    #region Protected Attributes
    protected AudioSource _audioSource;
    protected NavMeshAgent _navMesh;
    protected bool _goingToDestination = false;
    protected bool _isWalkingRandomly = true;
    protected float _currentSpeed;
    protected bool _isInfected;
    [SerializeField]
    protected float _walkSpeed = 4f;
    [SerializeField]
    protected float _runSpeed = 10f;
    [SerializeField]
    protected AudioClip[] _screamSfx;
    [SerializeField]
    protected bool _startWalkingRandomly = false;
    [SerializeField]
    protected float _minDistanceWhenDestinationReached = 1f;
    [SerializeField]
    protected Animator _animator;
    [SerializeField]
    protected float _checkingPeriod = .5f;
    [SerializeField]
    protected float _maxTimeScaredAndStuck = 2f;
    [SerializeField]
    protected bool _canBeInfected = true;
    #endregion

    #region Public Attributes
    public static bool canScream = true;
    public Vector3 currentDestination;
    #endregion

    public bool IsWalkingRandomly { get => _isWalkingRandomly; }
    public bool IsInfected { get => _isInfected; }

    protected void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _navMesh = GetComponent<NavMeshAgent>();
        _currentSpeed = _walkSpeed;
    }

    protected void Start()
    {
        LevelManager.instance.totalHumansInLevel++;

        _audioSource.volume = SoundManager.instance.CurrentVolume;
        _currentSpeed = _walkSpeed;

        if (_animator == null)
            throw new UnassignedReferenceException("Referencie o animator dentro do game object do humano (dentro do obj da mesh?).");

        if(_startWalkingRandomly)
            WalkRandomly();

        SetAnimationByName(GetIdleAnimation());
    }

    protected virtual void Update()
    {
        _checkingTimeCounter += Time.deltaTime;

        if (_goingToDestination)
        {
            _navMesh.SetDestination(currentDestination);
        }

        if (_checkingTimeCounter >= _checkingPeriod)
        {
            //Reached chacking period
            _checkingTimeCounter = 0f;

            if (_goingToDestination && Vector3.Distance(transform.position, currentDestination) <= _minDistanceWhenDestinationReached)
            {
                if(!IsInfected) {
                    WalkRandomly();
                } else
                {
                    Destroy(gameObject);
                }
            }
        }

        if(IsInfected)
        {
            if (!_navMesh.hasPath)
            {
                _timerScaredAndStuck += Time.deltaTime;

                if (_timerScaredAndStuck >= _maxTimeScaredAndStuck)
                {

                    Die();
                }
            } else
            {
                _timerScaredAndStuck = 0;
            }
        }
    }

    private string GetIdleAnimation()
    {
        return Utils.GetRandomArrayElement(idleAnimations);
    }

    private void GoToDestination(Vector3 destination)
    {
        currentDestination = destination;
        _goingToDestination = true;
    }

    private void SetCurrentSpeed(float speed)
    {
        _currentSpeed = speed;
        _navMesh.speed = _currentSpeed;
    }

    public virtual void Infect()
    {
        if (IsInfected) return;

        //SetAnimationByName(goofyRunningAnimation);
        SetPainted();
        //PlayScreamSfx();
        //Vector3 positionToRun = new Vector3(
        //    LevelManager.instance.GetLevelMaxX(),
        //    1.72f,
        //    LevelManager.instance.GetLevelMaxZ()
        //);
        //StopAndRunToDestination(positionToRun, .5f);
        GameObject humansSpawned = ObjectPooler.instance.Spawn("human-trio", transform.position, transform.rotation);
        if(humansSpawned != null)
        {

        }

        _isInfected = true;

        LevelManager.instance.IncrementHumansInfected();
    }

    public virtual void SetPainted()
    {

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

    public virtual void Stop()
    {
        _isWalkingRandomly = false;
        _goingToDestination = false;
        SetCurrentSpeed(0);
        SetAnimationByName(GetIdleAnimation());
    }

    public virtual void WalkRandomly()
    {
        _isWalkingRandomly = true;
        _goingToDestination = false;
        SetCurrentSpeed(_walkSpeed);
        GoToDestination(GetRandomDestination());
    }

    private Vector3 GetRandomDestination()
    {
        currentDestination = new Vector3(
            Utils.GetRandomFloatFromBounds(LevelManager.instance.GetLevelXBounds()),
            1.72f,
            Utils.GetRandomFloatFromBounds(LevelManager.instance.GetLevelZBounds())
        );
        return currentDestination;
    }

    public virtual void RunToPosition(Vector3 targetPosition, float delay = 0)
    {
        StartCoroutine(RunToPositionCo(targetPosition));
    }

    private IEnumerator RunToPositionCo(Vector3 targetPosition, float delay = 0)
    {
        _isWalkingRandomly = false;
        yield return new WaitForSeconds(delay);
        SetAnimationByName(runningAnimation);
        SetCurrentSpeed(_runSpeed);
        GoToDestination(targetPosition);
    }

    public void StopAndRunToDestination(Vector3 targetPosition, float stopDuration = 0)
    {
        StartCoroutine(StopAndRunToDestinationCo(targetPosition, stopDuration));
    }

    private IEnumerator StopAndRunToDestinationCo(Vector3 targetPosition, float stopDuration = 0)
    {
        _isWalkingRandomly = false;
        SetCurrentSpeed(0);
        yield return new WaitForSeconds(stopDuration);
        RunToPosition(targetPosition);
    }

    private void SetAnimationByName(string name)
    {
        _animator.Play(name);
    }

    public virtual void Die()
    {
        _isWalkingRandomly = false;
        Stop();
        Destroy(gameObject);
    }

    public bool IsRunning()
    {
        return _currentSpeed == _runSpeed;
    }

    public bool IsWalking()
    {
        return _currentSpeed == _walkSpeed;
    }

    public bool IsStopped()
    {
        return _currentSpeed == 0;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
