using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

        if (LevelManager.instance.quantitySlimeTactical <= 0 && LevelManager.instance.quantitySlimeCollector <= 0)
            LevelManager.instance.CreateGameOverEvent();
    }

    protected IEnumerator DamageArea(float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _decayRadius);

        if (colliders != null && colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                if (col.gameObject.CompareTag(GameManager.HUMAN_TAG))
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
        if (collision.gameObject.CompareTag(GameManager.HUMAN_TAG))
        {
            Human human = collision.gameObject.GetComponent<Human>();
            if (human != null)
            {
                human.rb.isKinematic = true;
                human.Infect(this);

                if(LevelManager.instance.OnGameOverEvent != null)
                    LevelManager.instance.OnGameOverEvent.Invoke();
                if(human.CanBeInfected)
                    CloneItSelf(_maxCloneCountOnHumans);
            }
        }
    }

    protected override void SetOnGroundMode()
    {
        base.SetOnGroundMode();
        StartCoroutine(DieCo());
    }

    private IEnumerator DieCo()
    {
        //Sync with the co-routine called in SetOnGroundMode()
        yield return new WaitForSeconds(10f);
        if (LevelManager.instance.isGameOver || LevelManager.instance.isLevelWon)
            Destroy(gameObject);
        else
            Die();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if ((_hasBeenLaunched || isClone) && collision != null && LevelManager.instance.IsGameActive())
        {
            if (CanDetectCollision())
            {
                if (!isGroundMode)
                {
                    PlayCollisionParticles();
                    //SoundManager.instance.PlaySound2D(Utils.GetRandomArrayElement(_collisionSfx));
                    GameManager.instance.VibrateAndShake();
                    StartCoroutine(DamageArea());
                    SetOnGroundMode();
                }

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
