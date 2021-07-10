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
    public Vector2 cannonLaunchForceLimits = new Vector2(50f,1000f);
    private float _currentLaunchForce;
    [SerializeField] private float _launchForceProgressBarVelocity = 2f;
    [SerializeField] private LaunchTrajectory _launchTrajectory;

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
            // Start varying force (show in the progress bar)
            //_countingLaunchForce = true;
            // Show the launch force bar
            //if(HUD.instance.HasSlimeSelected())
            //    HUD.instance.SetLaunchBarVisible(true);

            //_launchTrajectory.SetLineRendererSettings(launchPoint.position.y, 45f, _currentLaunchForce, touchLength);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            touchPosition = Input.GetTouch(0).position;
            hasStartedToAim = true;
            hasTouchedLevel = true;
            _countingLaunchForce = true;

            if(HUD.instance.HasSlimeSelected())
                HUD.instance.SetLaunchBarVisible(true);
        }      
#endif
        if (hasTouchedLevel)
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);

            RaycastHit hitInfo;

            if (Physics.Raycast(ray,out hitInfo))
            {
                pointToRay = new Vector3(hitInfo.point.x, launchPoint.position.y, hitInfo.point.z);

                float touchDistance = Mathf.Abs(pointToRay.z - launchPoint.position.z);
                float velocityNeededToReachThisDistance = _launchTrajectory.GetVelocityFromTargetDistance(touchDistance);
                print("velocityNeededToReachThisDistance = " + velocityNeededToReachThisDistance);

                SetLaunchForce(velocityNeededToReachThisDistance);
                SetLaunchTrajectorySettings();
                transform.LookAt(pointToRay);
            }
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject == null)
        {
            Vector3 from = new Vector3(launchPoint.position.x, launchPoint.position.y, launchPoint.position.z);
            Vector3 to = new Vector3(pointToRay.x, launchPoint.position.y, pointToRay.z);
            InstantiateSlime(Vector3.Normalize(to - from));
            //Launch Progress bar
            //_countingLaunchForce = false;
            //HUD.instance.SetLaunchBarVisible(false);
            _launchTrajectory.ClearLineRendererPoints();
        }
#else
        if (hasStartedToAim && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            hasStartedToAim = false;
            Vector3 from = new Vector3(launchPoint.position.x, launchPoint.position.y, launchPoint.position.z);
            Vector3 to = new Vector3(pointToRay.x, launchPoint.position.y, pointToRay.z);
            InstantiateSlime(to - from);
            _countingLaunchForce = false;
            HUD.instance.SetLaunchBarVisible(false);
        }  
#endif
    }

    private void SetLaunchTrajectorySettings()
    {
        _launchTrajectory.MaxXDistance = pointToRay.x;
        _launchTrajectory.X0 = launchPoint.position.x;
        _launchTrajectory.Y0 = launchPoint.position.y;
        _launchTrajectory.Z0 = launchPoint.position.z;
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
        if (HUD.instance.selectedSlime == null) return;

        Slime slimeInstantiated = Instantiate(HUD.instance.selectedSlime, launchPoint.position, Quaternion.identity).GetComponent<Slime>();

        if(slimeInstantiated != null)
        {
            slimeInstantiated.Launch(direction, _currentLaunchForce);
        }

        HUD.instance.ClearSelectedSlime();
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
