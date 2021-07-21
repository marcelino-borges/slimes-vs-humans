using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeTacticalAttractor : MonoBehaviour
{
    [SerializeField]
    private SlimeTactical slime;
    private SphereCollider sphereCollider;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();

        if(sphereCollider != null)
            sphereCollider.radius = slime.DecayRadius * 2f;
    }

    private void Start()
    {
        if (slime == null)
            throw new UnassignedReferenceException("Please reference the slime owner of this script (it should be in the parent game object");

        if (sphereCollider == null)
            throw new UnassignedReferenceException("There should be a sphere collider attached to this game object");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other != null)
        {
            if (other.gameObject.CompareTag("Human"))
            {
                HumanTacticalSlimeAttraction human = other.gameObject.GetComponent<HumanTacticalSlimeAttraction>();

                if (human != null)
                {
                    slime.OnDieEvent.AddListener(human.ClearAtraction);
                    human.SetAtraction(slime);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            if (other.gameObject.CompareTag("Human"))
            {
                HumanTacticalSlimeAttraction human = other.gameObject.GetComponent<HumanTacticalSlimeAttraction>();

                if (human != null)
                    human.ClearAtraction();
            }
        }
    }
}
