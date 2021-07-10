using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBomb : Slime
{
    [SerializeField] private float _launchHeightOffset = 10f;

    public override void Launch(Vector3 direction, float force)
    {        
        base.Launch(direction + new Vector3(0, _launchHeightOffset, 0), force);
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
}
