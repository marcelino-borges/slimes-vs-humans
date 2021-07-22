﻿using System.Collections;
using UnityEngine;

public class SlimeCollector : Slime
{
    protected override void Awake()
    {
        base.Awake();

        _slimeCloneType = SlimeType.COLLECTOR;
    }
    
    protected IEnumerator DamageArea(float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _decayRadius);

        if (colliders != null && colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                if (col.gameObject.CompareTag("Human"))
                {
                    Human human = col.gameObject.GetComponent<Human>();

                    if (human != null)
                    {
                        human.Infect(this);
                    }
                    CloneItSelf();
                }
            }
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (CanDetectCollision())
            {
                if (!isGroundMode)
                {
                    PlayCollisionParticles();
                    PlaySfx(Utils.GetRandomArrayElement(_collisionSfx));
                    Vibrate();
                }

                if (!isGroundMode)
                    StartCoroutine(DamageArea());

                SetOnGroundMode();

                //Collision against humans implemented in the IEnumerator DamageArea() in this class
                TestCollisionAgainstSlimes(collision);
                TestCollisionAgainstBuildings(collision);
                TestCollisionAgainstObstacles(collision);

                CountDetectCollisionCooldown();
            }
        }
    }
}
