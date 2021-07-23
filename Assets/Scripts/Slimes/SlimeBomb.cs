using System.Collections;
using UnityEngine;

public class SlimeBomb : Slime
{
    protected override void Awake()
    {
        base.Awake();

        _slimeCloneType = SlimeType.BOMB;
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Launch(Vector3 direction, Vector3 targetPosition, float force = 50)
    {
        base.Launch(direction, targetPosition, force);

        HUD.instance.cardSelected.DecrementQuantityLeft();
        LevelManager.instance.DecrementSlimeBomb();
    }

    protected IEnumerator DamageArea(float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _decayRadius);

        if(colliders != null && colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                if(col.gameObject.CompareTag("Building"))
                {
                    Building building = col.gameObject.transform.parent.gameObject.GetComponent<Building>(); //Gets Building script in parent GameObject

                    if (building != null)
                    {
                        building.Explode();
                    }
                }
            }
        }
        Die();
    }

    protected override void TestCollisionAgainstBuildings(Collision collision)
    {
        if (collision.gameObject.CompareTag("Building"))
        {
            Building building = collision.gameObject.transform.parent.gameObject.GetComponent<Building>(); //Gets Building script in parent GameObject

            if (building != null)
            {
                building.Explode();
                Die();
            }
        }
    }

    public override void Die()
    {
        if (_isDead) return;

        _isDead = true;
        Vibrate();

        _health = 0;
        PlayExplosionParticles();
        SoundManager.instance.PlaySound2D(_deathSfx);

        Destroy(gameObject);
    }

    protected override void TestCollisionAgainstObstacles(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
            obstacle.Explode();
            Die();
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            TestCollisionAgainstTerrain(collision);
            if (CanDetectCollision())
            {
                if (!isGroundMode)
                {
                    PlayCollisionParticles();
                    PlaySfx(Utils.GetRandomArrayElement(_collisionSfx));
                    Vibrate();
                }

                TestCollisionAgainstBuildings(collision);
                TestCollisionAgainstObstacles(collision);

                StartCoroutine(DamageArea(2f));
            }
        }
    }
}
