using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
public class Human : MonoBehaviour
{
    protected bool _isScared;
    protected float _currentSpeed;
    [SerializeField]
    protected float _walkSpeed = 4f;
    [SerializeField]
    protected float _runSpeed = 10f;
    [SerializeField]
    protected AudioClip[] _screamSfx;
    protected AudioSource _audioSource;
    protected NavMeshAgent _navMesh;
    public static bool canScream;
    public Vector3 currentDestination;
    protected bool _goingToDestination = false;
    [SerializeField]
    protected float _minDistanceWhenDestinationReached = 1f;
    [SerializeField]
    protected float _checkingPeriod = .5f;
    float _checkingTimeCounter = 0f;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        //_audioSource.volume = SoundManager.instance.CurrentVolume;

        _navMesh = GetComponent<NavMeshAgent>();
        _currentSpeed = _walkSpeed;
    }

    protected virtual void Start()
    {
        _audioSource.volume = SoundManager.instance.CurrentVolume;
        _currentSpeed = _walkSpeed;

        WalkRandomly();
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
            //Reached time
            _checkingTimeCounter = 0f;

            if (_goingToDestination && Vector3.Distance(transform.position, currentDestination) <= _minDistanceWhenDestinationReached)
            {
                WalkRandomly();
            }
        }
    }

    public void GoToDestination(Vector3 destination)
    {
        currentDestination = destination;
        _goingToDestination = true;
    }

    public void SetCurrentSpeed(float speed)
    {
        _currentSpeed = speed;
        _navMesh.speed = _currentSpeed;
    }

    public virtual void Scare()
    {
        _isScared = true;
        SetPainted();
        PlayScreamSfx();
        Vector3 positionToRun = new Vector3(
            LevelManager.instance.GetLevelMaxX(),
            1.72f,
            LevelManager.instance.GetLevelMaxZ()
        );

        StopAndRunToDestination(positionToRun);
    }

    public virtual bool IsScared()
    {
        return _isScared;
    }

    public virtual void SetPainted()
    {

    }

    private void PlayScreamSfx()
    {
        if (Utils.IsArrayValid(_screamSfx))
            _audioSource.PlayOneShot(Utils.GetRandomArrayElement(_screamSfx));
    }

    private IEnumerator CountScreamCooldown(float time = 3f)
    {
        yield return new WaitForSeconds(time);
    }

    public virtual void Stop()
    {
        _goingToDestination = false;
        SetCurrentSpeed(0);
    }

    public virtual void WalkRandomly()
    {
        print("WalkRandomly");
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
        yield return new WaitForSeconds(delay);
        SetCurrentSpeed(_runSpeed);
        GoToDestination(targetPosition);
    }

    public void StopAndRunToDestination(Vector3 targetPosition, float stopDuration = 0)
    {
        StartCoroutine(StopAndRunToDestinationCo(targetPosition, stopDuration));
    }

    private IEnumerator StopAndRunToDestinationCo(Vector3 targetPosition, float stopDuration = 0)
    {
        yield return new WaitForSeconds(stopDuration);
        RunToPosition(targetPosition);
    }

    public virtual void Die()
    {
        Stop();
        Destroy(gameObject);
    }

}
