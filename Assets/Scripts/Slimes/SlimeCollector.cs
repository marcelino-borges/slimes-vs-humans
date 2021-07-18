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

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (CanDetectCollision())
        {
            CountDetectCollisionCooldown();
            PlaySfx(Utils.GetRandomArrayElement(_collisionSfx));

            TestCollisionAgainstBuildings(collision);
            TestCollisionAgainstHumans(collision);
        }
    }
}
