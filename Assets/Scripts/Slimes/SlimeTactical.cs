using System.Collections;
using System.Collections.Generic;
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

    public override void Launch(Vector3 direction, Vector3 targetPosition, float force = 50f)
    {
#if UNITY_EDITOR
        //Debug only
        destinyPoint = transform.position + direction;
        originPoint = transform.position;
#endif
        velocity = direction * _launchForce;
        SetVelocity(velocity);
    }

    protected override void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
        base.SetVelocity(velocity);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (_canDetectCollision)
        {
            CountDetectCollisionCooldown();
            PlaySfx(Utils.GetRandomArrayElement(_collisionSfx));

            rb.drag = 2;
            rb.angularDrag = 2;

            if (collision.gameObject.CompareTag("Building"))
            {
                if (_collisionReflectionsCount <= _maxCollisionReflections)
                {
                    _collisionReflectionsCount++;

                    foreach (ContactPoint contact in collision.contacts)
                    {
                        Vector3 reflectedVelocity = velocity;
                        reflectedVelocity.x *= contact.normal.x != 0f ? -1 : 1;
                        reflectedVelocity.y *= 0;
                        reflectedVelocity.z *= contact.normal.z != 0f ? -1 : 1;

                        SetVelocity(reflectedVelocity);
                    }
                } else
                {
                    Die();
                }
            }
            if (collision.gameObject.CompareTag("Human"))
            {
                Human human = collision.gameObject.GetComponent<Human>();
                if (human != null)
                    human.Scare();
                CloneItSelf();
                Die();
            }

            if (collision.gameObject.CompareTag("Obstacle"))
            {
                Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
                obstacle.Explode();
                SetVelocity(Vector3.zero);
                Die();
            }
        }
    }
}
