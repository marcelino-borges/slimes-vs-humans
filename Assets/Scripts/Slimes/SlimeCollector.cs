using UnityEngine;

public class SlimeCollector : Slime, IPoolableObject
{
    protected override void Awake()
    {
        base.Awake();

        _slimeCloneType = SlimeType.COLLECTOR;
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

            _rb.drag = 2;
            _rb.angularDrag = 2;

            if (collision.gameObject.CompareTag("Building"))
            {
                PlayCollisionParticles();

                foreach (ContactPoint contact in collision.contacts)
                {
                    Vector3 reflectedVelocity = velocity;
                    reflectedVelocity.x *= contact.normal.x != 0f ? -1 : 1;
                    reflectedVelocity.y *= 0;
                    reflectedVelocity.z *= contact.normal.z != 0f ? -1 : 1;

                    SetVelocity(reflectedVelocity);
                }        
            }

            if (collision.gameObject.CompareTag("Human"))
            {
                PlayCollisionParticles();

                Human human = collision.gameObject.GetComponent<Human>();
                if (human != null)
                    human.Scare();
            }
        }
    }
}
