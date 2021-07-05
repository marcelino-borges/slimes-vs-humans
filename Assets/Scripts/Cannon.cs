using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public Vector3 pointToRay;

    void Start()
    {
        
    }

    void Update()
    {
        bool hasTouched = false;
        Vector3 touchPosition = Vector3.zero;

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            touchPosition = Input.mousePosition;
            hasTouched = true;
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            touchPosition = Input.GetTouch(0).position;
            hasTouched = true;
        }
#endif

        if (hasTouched)
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);

            RaycastHit hitInfo;

            if (Physics.Raycast(ray,out hitInfo))
            {
                pointToRay = hitInfo.point;
            }

            transform.LookAt(new Vector3(pointToRay.x, transform.position.y, pointToRay.z));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, pointToRay);
    }
}
