using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeTactical : Slime, IPoolableObject
{
    Vector3 velocity;

    public void OnSpawnedFromPool()
    {
        
    }

    public override void Launch(Vector3 direction, Vector3 targetPosition, float force = 50f, float radianAngle = 0)
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

            if (collision.gameObject.CompareTag("Building") && _collisionReflectionsCount <= _maxCollisionReflections)
            {
                PlayCollisionParticles();

                _collisionReflectionsCount++;

                foreach (ContactPoint contact in collision.contacts)
                {
                    print("contact.normal = " + contact.normal);

                    Vector3 reflectedVelocity = velocity;
                    reflectedVelocity.x *= contact.normal.x != 0 ? -1 : 1;
                    reflectedVelocity.y *= 0;
                    reflectedVelocity.z *= contact.normal.z != 0 ? -1 : 1;

                    print(" BEFORE: _rb.velocity = " + _rb.velocity);
                    print(" reflectedVelocity = " + reflectedVelocity);

                    SetVelocity(reflectedVelocity);
                    print(" AFTER: _rb.velocity = " + _rb.velocity);
                }

                Building building = collision.gameObject.transform.parent.gameObject.GetComponent<Building>(); //Gets Building script in parent GameObject

                if (building != null)
                {
                    building.Explode();
                }
            }

            if (collision.gameObject.CompareTag("Human"))
            {
                PlayCollisionParticles();

                Human human = collision.gameObject.GetComponent<Human>();
                if (human != null)
                    human.GetScared();
            }

            if (collision.gameObject.CompareTag("Obstacle"))
            {
                PlayCollisionParticles();

                Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
                obstacle.Explode();
            }
        }
    }
}
