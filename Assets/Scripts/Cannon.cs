using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public Vector3 pointToRay;
    private bool hasStartedToAim = false;
    public Transform launchPoint;

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
            hasStartedToAim = true;
            hasTouched = true;
        }
        if (Input.GetMouseButtonUp(0))
        {            
            hasStartedToAim = false;
            InstantiateSlime(pointToRay - transform.position);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            touchPosition = Input.GetTouch(0).position;
            hasTouched = true;
            hasStartedToAim = true;
        }

        if (hasStartedToAim && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            hasStartedToAim = false;
            InstantiateSlime(pointToRay - transform.position);
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

    private void InstantiateSlime(Vector3 direction)
    {
        if (HUD.instance.selectedSlime == null) return;

        SlimeBase slimeInstantiated = Instantiate(HUD.instance.selectedSlime, launchPoint.position, Quaternion.identity).GetComponent<SlimeBase>();

        if(slimeInstantiated != null)
        {
            slimeInstantiated.Launch(direction);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, pointToRay);
    }
}
