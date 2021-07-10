using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBomb : Slime
{
    bool _moving = false;
    Vector3 _positionOnLaunch = Vector3.zero;
    Vector3 _targetPosition = Vector3.zero;

    float time = 0;

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

    public override void Launch(Vector3 direction, Vector3 targetPosition, float force = 50f, float radianAngle = 0)
    {
        _positionOnLaunch = transform.position;
        this._targetPosition = targetPosition;  
        _launchForce = force;
        _moving = true;
#if UNITY_EDITOR
        //Debug only
        destinyPoint = transform.position + direction;
        originPoint = transform.position;
#endif      
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Building"))
            {
                print("acertou predio");
                Building building = collision.gameObject.transform.parent.gameObject.GetComponent<Building>(); //Gets Building script in parent GameObject

                if (building != null)
                {
                    building.Explode();
                }
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.gameObject.CompareTag("Building"))
            {
                print("acertou predio");
                Building building = other.gameObject.transform.parent.gameObject.GetComponent<Building>(); //Gets Building script in parent GameObject

                if (building != null)
                {
                    building.Explode();
                }
            }
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(originPoint, destinyPoint);
    }
#endif
}
