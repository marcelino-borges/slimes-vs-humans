using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBomb : Slime
{
    bool _moving = false;
    Vector3 _positionOnLaunch = Vector3.zero;
    Vector3 _targetPosition = Vector3.zero;

    float time = 0;
    protected override void Awake()
    {
        base.Awake();

        _slimeCloneType = SlimeType.BOMB;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected void FixedUpdate()
    {
        if(_moving)
        {
            time += Time.deltaTime;
            float x = time * _targetPosition.x;
            float z = time * _targetPosition.z;
            float y = Utils.GetYWhenAtZPosition(
                z,
                _launchForce, 
                LaunchTrajectory.degreeAngle * Mathf.Deg2Rad, 
                Mathf.Abs(Physics.gravity.y), 
                _positionOnLaunch.y
            );
            Vector3 nextPosition = new Vector3(x, y, z);
            _rb.MovePosition(nextPosition);
        }
    }

    public override void Launch(Vector3 direction, Vector3 targetPosition, float force = 50f)
    {
        _positionOnLaunch = transform.position;
        _targetPosition = targetPosition;  
        _launchForce = force;
        _moving = true;
        SetVelocity(direction * _launchForce);
#if UNITY_EDITOR
        //Debug only
        destinyPoint = transform.position + direction;
        originPoint = transform.position;
#endif      
    }

    protected override void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision != null && !collision.gameObject.CompareTag("Cannon"))
        {
            PlaySfx(Utils.GetRandomArrayElement(_collisionSfx));
            if (collision.gameObject.CompareTag("Building"))
            {
                Building building = collision.gameObject.transform.parent.gameObject.GetComponent<Building>(); //Gets Building script in parent GameObject

                if (building != null)
                {
                    building.Explode();
                    Die();                    
                }
            }
            StartCoroutine(DamageArea(2f));
            SetOnGroundMode();
        }
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

    public override void Die()
    {
        if (_isDead) return;

        _isDead = true;

        _health = 0;
        PlayExplosionParticles();

        Destroy(gameObject);
    }

    private void SetOnGroundMode()
    {
        _rb.useGravity = true;
        _moving = false;
        _rb.AddForce(velocity);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(originPoint, destinyPoint);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(originPoint, _decayRadius);
    }
#endif
}
