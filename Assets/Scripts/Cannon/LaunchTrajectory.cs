using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaunchTrajectory : MonoBehaviour
{
    [SerializeField] private float degreeAngle = 45;
    [SerializeField] private float velocity = 10f;
    public int lineResolution = 15; 

    LineRenderer line;
    float x0;
    float y0;
    float z0;
    float maxXDistance;
    float maxZDistance;

    float g;
    float radianAngle;

    public float Y0 { get => y0; set => y0 = value; }
    public float Z0 { get => z0; set => z0 = value; }
    public float X0 { get => x0; set => x0 = value; }
    public float MaxXDistance { get => maxXDistance; set => maxXDistance = value; }
    public float MaxZDistance { get => maxZDistance; set => maxZDistance = value; }
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

    private void SetMotionParameters(float y0, float degreeAngle, float velocity, float maxHorizontalDistance, int lineResolution = 15)
    {
        this.degreeAngle = degreeAngle;
        this.Velocity = velocity;
        this.lineResolution = lineResolution;
        this.Y0 = y0;
        this.MaxZDistance = maxHorizontalDistance;
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
        float y = GetYWhenAtZPosition(z);
        return new Vector3(x, y, z);
    }

    public float GetYWhenAtZPosition(float z)
    {
        return (z * Mathf.Tan(radianAngle)) - ((g * Mathf.Pow(z, 2)) / (2 * (Mathf.Pow((Velocity * Mathf.Cos(radianAngle)), 2))));
    }

    public float GetZMaxDistance()
    {
        return (Mathf.Pow(Velocity, 2) * Mathf.Sin(2 * radianAngle)) / g;
    }

    public float GetVelocityFromTargetDistance(float distance)
    {
        return Mathf.Sqrt((distance * g) / Mathf.Sin(2 * radianAngle));
    }

    public void ClearLineRendererPoints()
    {
        line.positionCount = 0;
    }
}
