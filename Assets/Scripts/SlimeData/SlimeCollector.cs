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
        _navMeshAgent.speed = Speed;
    }
    private void Update()
    {
        _navMeshAgent.SetDestination(_target.position);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Human")
        {
            print("human");
            CloneItSelf(this.gameObject, true);
            
        }
    }
    private void OnMouseDown()
    {
        Debug.Log("Click");
        CloneItSelf(this.gameObject, true);
    }
}
