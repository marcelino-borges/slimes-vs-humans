using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaunchTrajectory : MonoBehaviour
{
    public static float degreeAngle = 45;
    public float DegreeAngle { get => degreeAngle; set => degreeAngle = value; }
    [SerializeField] private float velocity = 10f;
    public int lineResolution = 15; 

    LineRenderer line;
    float maxXDistance;

    private Vector3 startPosition;
    public Vector3 StartPosition
    {
        get { return startPosition; }
        set { startPosition = value; }
    }

    private Vector3 endPosition;
    public Vector3 EndPosition
    {
        get { return endPosition; }
        set { endPosition = value; }
    }

    float g;
    float radianAngle;
    public float MaxXDistance { get => maxXDistance; set => maxXDistance = value; }
    public float Velocity { get => velocity; set => velocity = value; }

    protected void Awake()
    {
        line = GetComponent<LineRenderer>();
        g = Mathf.Abs(Physics.gravity.y);
    }

    public void SetLineRendererSettings()
    {
        if (line == null || !Application.isPlaying)
            return;

        line.positionCount = lineResolution + 1;
        line.SetPositions(CalculateLineRendererPoints());
    }

    public void SetMotionParameters(Vector3 startPosition, Vector3 endPosition, float velocity, int lineResolution = 15)
    {
        radianAngle = degreeAngle * Mathf.Deg2Rad;
        Velocity = velocity;
        this.lineResolution = lineResolution;
        StartPosition = startPosition;
        EndPosition = endPosition;
    }

    private Vector3[] CalculateLineRendererPoints()
    {
        Vector3[] linePositions = new Vector3[lineResolution + 1];
        radianAngle = Mathf.Deg2Rad * degreeAngle;
        float maxHorizontalDistance = GetZMaxDistance();

        for (int i = 0; i <= lineResolution; i++)
        {
            float t = (float)i / (float)lineResolution; // Não remover os "(float)"
            linePositions[i] = CalculateLinePosition(t, maxHorizontalDistance);
        }
        return linePositions;
    }

    private Vector3 CalculateLinePosition(float t, float maxZDistance)
    {
        float x = t * MaxXDistance;
        float z = t * maxZDistance;
        float y = GetYWhenAtZPosition(z, StartPosition.y);
        return new Vector3(x, y, z);
    }

    public float GetYWhenAtZPosition(float z, float y0)
    {
        return Utils.GetYWhenAtZPosition(z, Velocity, radianAngle, g);
    }

    public float GetZMaxDistance()
    {
        return Utils.GetZMaxDistance(Velocity, radianAngle, g);
    }

    public float GetVelocityNeededToReachDistance(float distance)
    {
        return Utils.GetVelocityNeededToReachDistance(distance, g, radianAngle);
    }

    public void ClearLineRendererPoints()
    {
        line.positionCount = 0;
    }
}
