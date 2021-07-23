using System.Collections;
using UnityEngine;

public class SlimeCollector : Slime
{
    protected override void Awake()
    {
        base.Awake();

        _slimeCloneType = SlimeType.COLLECTOR;
    }

    public override void Launch(Vector3 direction, Vector3 targetPosition, float force = 50)
    {
        base.Launch(direction, targetPosition, force);

        HUD.instance.cardSelected.DecrementQuantityLeft();
        LevelManager.instance.DecrementSlimeCollector();
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
                    CloneItSelf(_maxCloneCountOnHumans);
                }
            }
        }
    }

    protected override void TestCollisionAgainstHumans(Collision collision)
    {
        if (collision.gameObject.CompareTag("Human"))
        {
            Human human = collision.gameObject.GetComponent<Human>();
            if (human != null)
            {
                human.rb.isKinematic = true;
                human.Infect(this);
            }
            CloneItSelf(_maxCloneCountOnHumans);
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision != null && LevelManager.instance.IsGameActive())
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
                TestCollisionAgainstHumans(collision);

                CountDetectCollisionCooldown();
            }
        }
    }
}
