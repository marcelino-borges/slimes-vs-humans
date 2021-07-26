using System.Collections;
using UnityEngine;

public class SlimeTactical : Slime
{
    protected override void Awake()
    {
        base.Awake();

        _slimeCloneType = SlimeType.TACTICAL;
    }

    public override void Launch(Vector3 direction, Vector3 targetPosition, float force = 50)
    {
        base.Launch(direction, targetPosition, force);

        HUD.instance.cardSelected.DecrementQuantityLeft();
        LevelManager.instance.DecrementSlimeTactical();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (_hasBeenLaunched && collision != null)
        {
            if (CanDetectCollision())
            {
                if (!isGroundMode)
                {
                    PlayCollisionParticles();
                    //SoundManager.instance.PlaySound2D(Utils.GetRandomArrayElement(_collisionSfx));
                    GameManager.instance.VibrateAndShake();
                }

                SetOnGroundMode();

                if (!rb.isKinematic)
                    StartCoroutine(StopMovementsCo());

                TestCollisionAgainstObstacles(collision);
                TestCollisionAgainstBuildings(collision);

                CountDetectCollisionCooldown();
            }
        }
    }

    private IEnumerator StopMovementsCo()
    {
        yield return new WaitForSeconds(1.5f);
        rb.isKinematic = true;
        StartCoroutine(DieCo());
    }

    private IEnumerator DieCo()
    {
        yield return new WaitForSeconds(2f);
        Die();
    }
}
