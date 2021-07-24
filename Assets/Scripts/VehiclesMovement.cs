using UnityEngine;

public class VehiclesMovement : MonoBehaviour
{
    [Space(5), Tooltip("If equal to zero, the vehicle will have no movement"), SerializeField]
    private float velocity;

    [Space(5), Tooltip("Destiny of the movement of the vehicle"), SerializeField]
    private Transform destinyPoint;

    private Vector3 vectorDestiny;
    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
        vectorDestiny = new Vector3(destinyPoint.localPosition.x, destinyPoint.localPosition.y, destinyPoint.localPosition.z);
    }

    private void FixedUpdate()
    {
        if (velocity != 0)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, originalPosition + vectorDestiny, Mathf.PingPong(Time.time, 1));
        }
    }
}
