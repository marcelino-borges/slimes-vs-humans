using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeCollector : SlimeBase
{
    [Space(10f)]
    [Header("Expecific Attributes")]
    [SerializeField] private string _details;
    [SerializeField] private Transform _target;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        _navMeshAgent.SetDestination(_target.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Humans")
        {
            print("human");
            Explode();
        }
    }
    private void OnMouseDown()
    {
        Debug.Log("Click");
        CloneItSelf(this.gameObject, true);
    }
}
