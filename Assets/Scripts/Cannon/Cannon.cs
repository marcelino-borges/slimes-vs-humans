using MilkShake;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cannon : MonoBehaviour
{
    [SerializeField] 
    private Transform crossMarkInLevel;
    private float _currentLaunchForce = 25f;
    private LaunchTrajectory _launchTrajectory;
    private Slime _slimeInstantiated;
    private bool hasTouched;

    public Transform launchPoint;
    public ShakePreset shakePreset;
    public float aimingSensitivity = 3f;

    protected void Awake()
    {
        _launchTrajectory = GetComponent<LaunchTrajectory>();
    }

    protected void Start()
    {
        // Start current force as the minimum value
        SetLaunchForce(_currentLaunchForce);
        ResetCrossMarkPosition();
    }

    protected void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            print("_currentGlobalClonesCount = " + Slime._currentGlobalClonesCount);
        }
#endif
        hasTouched = false;

#if UNITY_EDITOR
        if (Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject == null)
        {
            //&& EventSystem.current.currentSelectedGameObject == null      --->     No UI element clicked
            hasTouched = true;
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && EventSystem.current.currentSelectedGameObject == null)
        {
            hasTouched = true;
        }      
#endif
        if (hasTouched && HUD.instance.HasSlimeSelected())
        {
            if (_slimeInstantiated == null)
                _slimeInstantiated = Instantiate(HUD.instance.selectedSlime, launchPoint.position, Quaternion.identity).GetComponent<Slime>();

            IncrementCrossMarkPosition(aimingSensitivity * new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y")));
            CalculateVelocityToReachTouchedPoint(crossMarkInLevel.position);
            transform.LookAt(crossMarkInLevel.position);
            SetLaunchTrajectorySettings();

            Vector3 crossMarkUpOffset = crossMarkInLevel.position + new Vector3(0, 25f, 0);

            if (Physics.Raycast(crossMarkUpOffset, crossMarkInLevel.position - crossMarkUpOffset, out RaycastHit hit))
            {
                SetCrossMarkPosition(new Vector3(crossMarkInLevel.position.x, hit.point.y + .2f, crossMarkInLevel.position.z));
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
            SetSlimeLaunch();
        }
#endif
    }

    private void SetSlimeLaunch()
    {
        //Vector3 from = new Vector3(launchPoint.position.x, launchPoint.position.y, launchPoint.position.z);
        Vector3 from = new Vector3(launchPoint.position.x, launchPoint.position.y, launchPoint.position.z);
        Vector3 to = new Vector3(
            crossMarkInLevel.transform.position.x,
            launchPoint.position.y,
            crossMarkInLevel.transform.position.z
        );

        if (HUD.instance.HasSlimeSelected())
            LaunchSlime(Vector3.Normalize(to - from));

        ResetCrossMarkPosition();
        _launchTrajectory.ClearLineRendererPoints();
    }

    private void LaunchSlime(Vector3 direction)
    {
        if (_slimeInstantiated != null)
        {
            _slimeInstantiated.Launch(direction, crossMarkInLevel.transform.position, _currentLaunchForce);
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
        _launchTrajectory.SetMotionParameters(launchPoint.position, crossMarkInLevel.transform.position, _currentLaunchForce);
        _launchTrajectory.MaxXDistance = crossMarkInLevel.transform.position.x;
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
            crossMarkInLevel.position = new Vector3(position.x, position.y, position.z);            
        }
    }

    private void IncrementCrossMarkPosition(Vector3 increment)
    {
        SetCrossMarkPosition(crossMarkInLevel.position + increment);
    }

    private void ResetCrossMarkPosition()
    {
        if (crossMarkInLevel != null)
        {
            crossMarkInLevel.transform.position = new Vector3(
                TerrainRotation.instance.transform.position.x,
                crossMarkInLevel.position.y,
                TerrainRotation.instance.transform.position.z
            );
            crossMarkInLevel.gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(launchPoint.position, crossMarkInLevel.transform.position);
        Gizmos.DrawSphere(launchPoint.position, .25f);
    }
#endif
}
