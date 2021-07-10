using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cannon : MonoBehaviour
{
    public Vector3 pointToRay;
    public Transform launchPoint;
    private bool _countingLaunchForce;
    private bool _countingLaunchForceUp = true;
    [MinMaxSlider(0f, 1000f)]
    public Vector2 cannonLaunchForceLimits = new Vector2(50f, 1000f);
    private float _currentLaunchForce;
    [SerializeField] private float _launchForceProgressBarVelocity = 2f;
    [SerializeField] private LaunchTrajectory _launchTrajectory;
    [SerializeField] private Vector3 crossMarkOriginalPosition = new Vector3(-200f, 1.75f, -200f);
    [SerializeField] private Transform crossMarkInLevel;

    protected void Awake()
    {
        _launchTrajectory = GetComponent<LaunchTrajectory>();
    }

    protected void Start()
    {
        // Start current force as the minimum value
        SetLaunchForce(cannonLaunchForceLimits.x);
        HUD.instance.SetLaunchBarLimits(cannonLaunchForceLimits);
    }

    protected void Update()
    {
        //if (_countingLaunchForce)
        //{
        //    if(_countingLaunchForceUp)
        //    {
        //        SetLaunchForce(_currentLaunchForce + (Time.deltaTime * _launchForceProgressBarVelocity));

        //        if (_currentLaunchForce >= cannonLaunchForceLimits.y)
        //        {
        //            SetLaunchForce(cannonLaunchForceLimits.y);
        //            _countingLaunchForceUp = false;
        //        }
        //    } else
        //    {
        //        SetLaunchForce(_currentLaunchForce - (Time.deltaTime * _launchForceProgressBarVelocity));

        //        if (_currentLaunchForce <= cannonLaunchForceLimits.x)
        //        {
        //            SetLaunchForce(cannonLaunchForceLimits.x);
        //            _countingLaunchForceUp = true;
        //        }
        //    }
        //}

        bool hasTouchedLevel = false;
        Vector3 touchPosition = Vector3.zero;

#if UNITY_EDITOR
        if (Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject == null)
        {
            //  && EventSystem.current.currentSelectedGameObject == null      --->     No UI element clicked
            touchPosition = Input.mousePosition;
            hasTouchedLevel = true;
            //ShowLaunchBar(true);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            touchPosition = Input.GetTouch(0).position;
            hasStartedToAim = true;
            hasTouchedLevel = true;
            //ShowLaunchBar(true);
        }      
#endif
        if (hasTouchedLevel && HUD.instance.HasSlimeSelected())
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);

            RaycastHit hitInfo;

            if (Physics.Raycast(ray,out hitInfo))
            {
                pointToRay = new Vector3(hitInfo.point.x, launchPoint.position.y, hitInfo.point.z);
                CalculateVelocityToReachTouchedPoint(pointToRay);

                SetCrossMarkPosition(pointToRay);

                SetLaunchTrajectorySettings();
                transform.LookAt(pointToRay);
            }
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject == null)
        {
            Vector3 from = new Vector3(launchPoint.position.x, launchPoint.position.y, launchPoint.position.z);
            Vector3 to = new Vector3(pointToRay.x, launchPoint.position.y, pointToRay.z);

            if(HUD.instance.HasSlimeSelected())
                InstantiateSlime(Vector3.Normalize(to - from));

            //ShowLaunchBar(false);
            ResetSetCrossMarkPosition();
            _launchTrajectory.ClearLineRendererPoints();
        }
#else
        if (hasStartedToAim && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Vector3 from = new Vector3(launchPoint.position.x, launchPoint.position.y, launchPoint.position.z);
            Vector3 to = new Vector3(pointToRay.x, launchPoint.position.y, pointToRay.z);

            if(HUD.instance.HasSlimeSelected())
                InstantiateSlime(Vector3.Normalize(to - from));

            //ShowLaunchBar(false);
            ResetSetCrossMarkPosition();
            _launchTrajectory.ClearLineRendererPoints();
        }  
#endif
    }

    private void ShowLaunchBar(bool visible)
    {
        _countingLaunchForce = visible;
        
        if (HUD.instance.HasSlimeSelected())
            HUD.instance.SetLaunchBarVisible(visible);
    }

    private void CalculateVelocityToReachTouchedPoint(Vector3 point)
    {
        float touchDistance = Mathf.Abs(point.z - launchPoint.position.z);
        float velocityNeededToReachThisDistance = _launchTrajectory.GetVelocityNeededToReachDistance(touchDistance);
        SetLaunchForce(velocityNeededToReachThisDistance);
    }

    private void SetLaunchTrajectorySettings()
    {
        _launchTrajectory.SetMotionParameters(launchPoint.position, pointToRay, _currentLaunchForce);
        _launchTrajectory.MaxXDistance = pointToRay.x;
        _launchTrajectory.SetLineRendererSettings();
    }

    private void SetLaunchForce(float value)
    {
        _currentLaunchForce = value;
        _launchTrajectory.Velocity = value;
        HUD.instance.SetLaunchBarValue(value);
    }

    private void InstantiateSlime(Vector3 direction)
    {
        Slime slimeInstantiated = Instantiate(HUD.instance.selectedSlime, launchPoint.position, Quaternion.identity).GetComponent<Slime>();

        if(slimeInstantiated != null)
        {
            slimeInstantiated.Launch(direction, pointToRay, _currentLaunchForce, LaunchTrajectory.degreeAngle * Mathf.Deg2Rad);
        }

        HUD.instance.ClearSelectedSlime();
    }

    private void SetCrossMarkPosition(Vector3 position)
    {
        //keeping Y position (Terrain fixed position)
        if (crossMarkInLevel != null)
        {
            crossMarkInLevel.gameObject.SetActive(true);
            crossMarkInLevel.position = new Vector3(position.x, crossMarkInLevel.position.y, position.z);            
        }
    }

    private void ResetSetCrossMarkPosition()
    {
        if (crossMarkInLevel != null)
            crossMarkInLevel.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(launchPoint.position, pointToRay);
        Gizmos.DrawSphere(launchPoint.position, .25f);
    }
#endif
}
