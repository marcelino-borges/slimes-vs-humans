using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Human : MonoBehaviour
{
    protected bool isScared;
    protected float currentMoveSpeed;
    protected float walkSpeed;
    protected float runSpeed;
    protected AudioClip[] screamSfx;
    protected AudioSource audioSource;
    public static bool canScream;

    protected virtual void Start()
    {
        audioSource.volume = SoundManager.instance.CurrentVolume;
    }

    protected virtual void Update()
    {
        
    }

    public virtual void GetScared()
    {
        isScared = true;
        SetPainted();
        Vector3 postionToRun = GetPositionToRunTo();
        RunToPosition(postionToRun);
        Die();
    }

    public virtual bool IsScared()
    {
        return isScared;
    }

    public virtual void SetPainted()
    {

    }

    public virtual Vector3 GetPositionToRunTo()
    {
        return Vector3.zero;
    }

    public virtual void PlayScreamSfx()
    {
        if (Utils.IsArrayValid(screamSfx))
            audioSource.PlayOneShot(Utils.GetRandomArrayElement(screamSfx));
    }

    public virtual IEnumerator CountScreamCooldown(float time = 3f)
    {
        yield return new WaitForSeconds(time);
    }

    public virtual void Stop()
    {
        currentMoveSpeed = 0;
    }

    public virtual void WalkRandomly()
    {
        currentMoveSpeed = walkSpeed;
    }

    public virtual void RunToPosition(Vector3 targetPostion)
    {
        StartCoroutine(RunToPositionCo(targetPostion));
    }

    public virtual IEnumerator RunToPositionCo(Vector3 targetPostion, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        currentMoveSpeed = runSpeed;
    }

    public virtual void Die()
    {
        Stop();
        Destroy(gameObject);
    }

}
