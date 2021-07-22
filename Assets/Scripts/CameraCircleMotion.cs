using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCircleMotion : MonoBehaviour
{
    [SerializeField]
    private Transform rotationCenter;
    [SerializeField]
    private float rotationRadius = 2f;
    [SerializeField]
    private float angularSpeed = 3f;

    private float x = 0f, y = 0f, z = 0f, angle = 0f;

    private void Start()
    {
        y = transform.position.y;
    }

    void Update()
    {
        transform.LookAt(rotationCenter.position, Vector3.up);
        x = rotationCenter.position.x + Mathf.Cos(angle) * rotationRadius;
        z = rotationCenter.position.z + Mathf.Sin(angle) * rotationRadius;
        transform.position = new Vector3(x, y, z);
        angle += Time.smoothDeltaTime * angularSpeed;

        if (angle >= 360f)
            angle = 0;
    }
}
