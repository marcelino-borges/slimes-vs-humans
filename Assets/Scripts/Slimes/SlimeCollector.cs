using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeCollector : Slime
{
    Vector3 velocity;
    
    public override void Launch(Vector3 direction, Vector3 targetPosition, float force = 50f, float radianAngle = 0)
    {
#if UNITY_EDITOR
        //Debug only
        destinyPoint = transform.position + direction;
        originPoint = transform.position;
#endif
        print("direction collector: " + direction);
        velocity = direction * force * 1.5f;
        SetVelocity(velocity);
    }

    protected override void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
        base.SetVelocity(velocity);        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Building"))
        {
            foreach(ContactPoint contact in collision.contacts)
            {
                //Find the BOUNCE of the object
                velocity = 2 * (Vector3.Dot(velocity, Vector3.Normalize(contact.normal))) * Vector3.Normalize(contact.normal) - velocity; //Following formula  v' = 2 * (v . n) * n - v
            }            
            //CloneItSelf(gameObject, true);            
        }

        if(collision.gameObject.CompareTag("Human"))
        {
            Human human = collision.gameObject.GetComponent<Human>();
            if(human != null)
                human.GetScared();
        }
    }
}
