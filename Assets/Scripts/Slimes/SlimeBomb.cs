using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBomb : Slime
{
    [SerializeField] private float _launchHeightOffset = 10f;
    bool moving = false;
    Vector3 positionOnLaunch = Vector3.zero;
    Vector3 targetPosition = Vector3.zero;

    float time = 0;

    protected void FixedUpdate()
    {
        if(moving)
        {
            time += Time.deltaTime;
            float x = time * targetPosition.x;
            float z = time * targetPosition.z;
            float y = Utils.GetYWhenAtZPosition(
                z,
                _launchForce, 
                LaunchTrajectory.degreeAngle * Mathf.Deg2Rad, 
                Mathf.Abs(Physics.gravity.y), 
                positionOnLaunch.y
            );
            Vector3 nextPosition = new Vector3(x, y, z);
            _rb.MovePosition(nextPosition);
        }
    }

    public override void Launch(Vector3 direction, Vector3 targetPosition, float force = 50f, float radianAngle = 0)
    {
        //base.Launch(direction + new Vector3(0, _launchHeightOffset, 0), force);
        positionOnLaunch = transform.position;
        this.targetPosition = targetPosition;
        print("force of launch = " + force);
#if UNITY_EDITOR
        //Debug only
        destinyPoint = transform.position + direction;
        originPoint = transform.position;
#endif        
        _launchForce = force;
        moving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.gameObject.CompareTag("Building"))
            {
                print("hit building");
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
    Vector3 destinyPoint = Vector3.zero;
    Vector3 originPoint = Vector3.zero;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(originPoint, destinyPoint);
    }
#endif
}
