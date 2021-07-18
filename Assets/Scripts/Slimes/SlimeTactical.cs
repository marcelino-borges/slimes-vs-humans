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

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (CanDetectCollision())
        {
            CountDetectCollisionCooldown();
            PlaySfx(Utils.GetRandomArrayElement(_collisionSfx));

            TestCollisionAgainstBuildings(collision);
            TestCollisionAgainstHumans(collision);
            TestCollisionAgainstObstacles(collision);
        }
    }
}
