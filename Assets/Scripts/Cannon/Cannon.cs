using MilkShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cannon : MonoBehaviour
{
    [SerializeField] 
    private Transform crossMarkInLevel;
    private float _currentLaunchForce = 25f;
    private LaunchTrajectory _launchTrajectory;
    private Slime _slimeInstantiated;

    public Vector3 pointToRay;
    public Transform launchPoint;
    public ShakePreset shakePreset;

    protected void Awake()
    {
        _launchTrajectory = GetComponent<LaunchTrajectory>();
    }

    protected void Start()
    {
        // Start current force as the minimum value
        SetLaunchForce(_currentLaunchForce);
    }

    protected void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            print("_currentGlobalClonesCount = " + Slime._currentGlobalClonesCount);
        }
#endif
        bool hasTouchedLevel = false;
        Vector3 touchPosition = Vector3.zero;

#if UNITY_EDITOR
        if (Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject == null)
        {
            //&& EventSystem.current.currentSelectedGameObject == null      --->     No UI element clicked
            touchPosition = Input.mousePosition;
            hasTouchedLevel = true;
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && EventSystem.current.currentSelectedGameObject == null)
        {
            touchPosition = Input.GetTouch(0).position;
            hasTouchedLevel = true;
        }      
#endif
        if (hasTouchedLevel && HUD.instance.HasSlimeSelected())
        {
            if (_slimeInstantiated == null)
                _slimeInstantiated = Instantiate(HUD.instance.selectedSlime, launchPoint.position, Quaternion.identity).GetComponent<Slime>();

            Ray ray = Camera.main.ScreenPointToRay(touchPosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
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
            SetSlimeLaunch();
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && EventSystem.current.currentSelectedGameObject == null)
        {
            LaunchSlime();
        }
#endif
    }

    private void SetSlimeLaunch()
    {
        Vector3 from = new Vector3(launchPoint.position.x, launchPoint.position.y, launchPoint.position.z);
        Vector3 to = new Vector3(pointToRay.x, launchPoint.position.y, pointToRay.z);

        if (HUD.instance.HasSlimeSelected())
            LaunchSlime(Vector3.Normalize(to - from));

        ResetCrossMarkPosition();
        _launchTrajectory.ClearLineRendererPoints();
    }

    private void LaunchSlime(Vector3 direction)
    {
        if (_slimeInstantiated != null)
        {
            _slimeInstantiated.Launch(direction, pointToRay, _currentLaunchForce);
            LevelManager.instance.IncrementSlimeLaunched();
            _slimeInstantiated = null;
        }

        HUD.instance.ClearSelectedSlime();
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

    private void SetCrossMarkPosition(Vector3 position)
    {
        //keeping Y position (Terrain fixed position)
        if (crossMarkInLevel != null)
        {
            crossMarkInLevel.gameObject.SetActive(true);
            crossMarkInLevel.position = new Vector3(position.x, crossMarkInLevel.position.y, position.z);            
        }
    }

    private void ResetCrossMarkPosition()
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
