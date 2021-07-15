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

        if (CanDetectCollision())
        {
            CountDetectCollisionCooldown();
            PlaySfx(Utils.GetRandomArrayElement(_collisionSfx));

            rb.drag = 2;
            rb.angularDrag = 2;
            TestCollisionAgainstBuildings(collision);
            TestCollisionAgainstHumans(collision);
        }
    }
}
