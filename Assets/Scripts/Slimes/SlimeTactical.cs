using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeTactical : Slime, IPoolableObject
{
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

            if (collision.gameObject.CompareTag("Building"))
            {
                if (_collisionReflectionsCount <= _maxCollisionReflections)
                {
                    PlayCollisionParticles();

                    _collisionReflectionsCount++;

                    foreach (ContactPoint contact in collision.contacts)
                    {
                        //print("BEFORE REFLECTION:" +
                        //    "\n\n1) contact.normal = " + contact.normal + 
                        //    "\n2) _rb.velocity = " + _rb.velocity + 
                        //    "\n3) velocity = " + velocity);

                        Vector3 reflectedVelocity = velocity;
                        reflectedVelocity.x *= contact.normal.x != 0f ? -1 : 1;
                        reflectedVelocity.y *= 0;
                        reflectedVelocity.z *= contact.normal.z != 0f ? -1 : 1;

                        SetVelocity(reflectedVelocity);

                        //print("AFTER REFLECTION:" +
                        //    "\n\n1) reflectedVelocity = " + reflectedVelocity +
                        //    "\n2) _rb.velocity = " + _rb.velocity);
                    }
                } else
                {
                    Die();
                }
            }

            if (collision.gameObject.CompareTag("Human"))
            {
                PlayCollisionParticles();

                Human human = collision.gameObject.GetComponent<Human>();
                if (human != null)
                    human.Scare();
            }

            if (collision.gameObject.CompareTag("Obstacle"))
            {
                PlayCollisionParticles();

                Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
                obstacle.Explode();
                SetVelocity(Vector3.zero);
                Die();
            }
        }
    }
}
