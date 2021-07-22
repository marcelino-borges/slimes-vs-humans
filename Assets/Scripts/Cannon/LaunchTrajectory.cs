using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaunchTrajectory : MonoBehaviour
{
    private float _g;
    private float _radianAngle;
    private LineRenderer _line;
    private float _maxXDistance;
    private Vector3 _endPosition;
    private Vector3 _startPosition;
    [SerializeField] private float _velocity = 10f;

    public static float degreeAngle = 45;
    public float DegreeAngle { get => degreeAngle; set => degreeAngle = value; }
    public int lineResolution = 15; 

    public Vector3 StartPosition
    {
        get { return _startPosition; }
        set { _startPosition = value; }
    }

    public Vector3 EndPosition
    {
        get { return _endPosition; }
        set { _endPosition = value; }
    }
    public float MaxXDistance { get => _maxXDistance; set => _maxXDistance = value; }
    public float Velocity { get => _velocity; set => _velocity = value; }

    protected void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _g = Mathf.Abs(Physics.gravity.y);
    }

    public void SetLineRendererSettings()
    {
        if (_line == null || !Application.isPlaying)
            return;

        _line.positionCount = lineResolution + 1;
        _line.SetPositions(CalculateLineRendererPoints());
    }

    public void SetMotionParameters(Vector3 startPosition, Vector3 endPosition, float velocity, int lineResolution = 15)
    {
        _radianAngle = degreeAngle * Mathf.Deg2Rad;
        Velocity = velocity;
        this.lineResolution = lineResolution;
        StartPosition = startPosition;
        EndPosition = endPosition;
    }

    private Vector3[] CalculateLineRendererPoints()
    {
        Vector3[] linePositions = new Vector3[lineResolution + 1];
        _radianAngle = Mathf.Deg2Rad * degreeAngle;
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
        float x = _startPosition.x + t * MaxXDistance;
        float z = _startPosition.z + t * maxZDistance;
        float y = GetYWhenAtZPosition(z, _startPosition.y);
        return new Vector3(x, y, z);
    }

    public float GetYWhenAtZPosition(float z, float y0 = 0)
    {
        return Utils.GetYWhenAtZPosition(z, Velocity, _radianAngle, _g, y0);
    }

    public float GetZMaxDistance()
    {
        return Utils.GetZMaxDistance(Velocity, _radianAngle, _g, _startPosition.z);
    }

    public float GetVelocityNeededToReachDistance(float distance)
    {
        return Utils.GetVelocityNeededToReachDistance(distance, _g, _radianAngle);
    }

    public void ClearLineRendererPoints()
    {
        _line.positionCount = 0;
    }
}
