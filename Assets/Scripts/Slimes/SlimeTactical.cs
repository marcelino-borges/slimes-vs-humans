using System;
using System.Collections;
using UnityEngine;

public class SlimeTactical : Slime, IPoolableObject
{
    protected override void Awake()
    {
        base.Awake();

        _slimeCloneType = SlimeType.TACTICAL;
    }

    public void OnSpawnedFromPool()
    {

    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (CanDetectCollision())
            {
                PlayExplosionParticles();
                PlayCollisionParticles();

                SetOnGroundMode();

                if (!rb.isKinematic)
                    StartCoroutine(StopMovementsCo());

                TestCollisionAgainstObstacles(collision);
                TestCollisionAgainstBuildings(collision);
                TestCollisionAgainstSlimes(collision);

                CountDetectCollisionCooldown();
                PlaySfx(Utils.GetRandomArrayElement(_collisionSfx));
            }
        }
    }

    private IEnumerator StopMovementsCo()
    {
        yield return new WaitForSeconds(1.5f);
        rb.isKinematic = true;
        StartCoroutine(DieCo());
    }

    private IEnumerator DieCo()
    {
        yield return new WaitForSeconds(2f);
        Die();
    }
}
