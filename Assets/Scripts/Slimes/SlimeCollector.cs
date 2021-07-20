using System.Collections;
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

    protected IEnumerator DamageArea(float delay)
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
                        human.rb.isKinematic = true;
                        human.Infect(this);
                    }
                }
            }
        }
        Die();
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

                //Collision against humans implemented in the IEnumerator DamageArea() in this class
                TestCollisionAgainstSlimes(collision);
                TestCollisionAgainstBuildings(collision);
                TestCollisionAgainstObstacles(collision);

                CountDetectCollisionCooldown();
                PlaySfx(Utils.GetRandomArrayElement(_collisionSfx));

                StartCoroutine(DamageArea(0));
            }
        }
    }
}
