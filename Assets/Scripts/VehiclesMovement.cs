using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclesMovement : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private Transform destinyPoint;

    private Vector3 vectorDestiny;

    private void Start() => vectorDestiny = new Vector3(destinyPoint.localPosition.x, destinyPoint.localPosition.y, destinyPoint.localPosition.z);

    private void FixedUpdate() => transform.localPosition = Vector3.MoveTowards(transform.localPosition, vectorDestiny, velocity);
}
