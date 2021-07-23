using MilkShake;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cannon : MonoBehaviour
{
    [SerializeField] 
    private Transform crossMarkInLevel;
    private SpriteRenderer _crossMarkSprite;
    private float _currentLaunchForce = 25f;
    private LaunchTrajectory _launchTrajectory;
    private bool _hasTouched;
    private bool _isTargetValid = true;
    [SerializeField]
    private AudioClip invalidTargetSfx;

    public Vector3 crossMarkInitialPosition;
    public Slime slimeInstantiated;
    public Transform launchPoint;
    public ShakePreset shakePreset;
    public float aimingSensitivity = 3f;

    protected void Awake()
    {
        _launchTrajectory = GetComponent<LaunchTrajectory>();
        
        if(crossMarkInLevel != null)
            _crossMarkSprite = crossMarkInLevel.gameObject.GetComponent<SpriteRenderer>();
    }

    protected void Start()
    {
        // Start current force as the minimum value
        crossMarkInitialPosition = new Vector3(
            TerrainRotation.instance.transform.position.x,
            crossMarkInLevel.position.y,
            TerrainRotation.instance.transform.position.z
        );
        crossMarkInLevel.transform.position = crossMarkInitialPosition;
        SetLaunchForce(_currentLaunchForce);
        ResetCrossMarkPosition();
    }

    protected void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            print("_currentGlobalClonesCount = " + Slime.currentGlobalClonesCount);
        }
#endif
        _hasTouched = false;

#if UNITY_EDITOR
        if (Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject == null)
        {
            //&& EventSystem.current.currentSelectedGameObject == null      --->     No UI element clicked
            _hasTouched = true;
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && EventSystem.current.currentSelectedGameObject == null)
        {
            hasTouched = true;
        }      
#endif
        if (_hasTouched && HUD.instance.HasSlimeSelected())
        {
            IncrementCrossMarkPosition(aimingSensitivity * new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y")));
            CalculateVelocityToReachTouchedPoint(crossMarkInLevel.position);
            SetLaunchTrajectorySettings();
            //transform.LookAt(crossMarkInLevel.position);

            Vector3 yOffsetFromCrossMark = crossMarkInLevel.position + new Vector3(0, 25f, 0);

            if (Physics.Raycast(yOffsetFromCrossMark, crossMarkInLevel.position - yOffsetFromCrossMark, out RaycastHit hit))
            {
                if (!hit.collider.gameObject.CompareTag("Water"))
                {
                    _isTargetValid = true;
                    Vector3 pos = new Vector3(crossMarkInLevel.position.x, hit.point.y + .2f, crossMarkInLevel.position.z);
                    SetCrossMarkPosition(pos);
                    SetCrossMarkColor(Color.white);
                } else
                {
                    _isTargetValid = false;
                    SetCrossMarkColor(Color.red);
                }
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

    protected virtual void ShakeCamera()
    {
        if (shakePreset != null)
            Shaker.ShakeAll(shakePreset);
    }

    private void SetSlimeLaunch()
    {
        if (!_isTargetValid)
        {
            _launchTrajectory.ClearLineRendererPoints();
            crossMarkInLevel.gameObject.SetActive(false);
            ShakeCamera();
            SoundManager.instance.PlaySound2D(invalidTargetSfx);
            return;
        }

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
        if (slimeInstantiated != null)
        {
            slimeInstantiated.Launch(direction, crossMarkInLevel.transform.position, _currentLaunchForce);
            LevelManager.instance.IncrementSlimeLaunched();
            slimeInstantiated = null;
        }
        //HUD.instance.ClearSelectedSlime();
        //HUD.instance.SelectSlime(HUD.instance.selectedSlime);
        InstantiateSlime(HUD.instance.selectedSlime, 1f);
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

    public void SetCrossMarkPosition(Vector3 position)
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

    public void SetCrossMarkVisibleInInitialPosition()
    {
        if (crossMarkInLevel != null)
        {
            crossMarkInLevel.transform.position = crossMarkInitialPosition;
            crossMarkInLevel.gameObject.SetActive(true);
        }
    }

    private void SetCrossMarkColor(Color color)
    {
        _crossMarkSprite.color = color;
    }

    public void InstantiateSlime(GameObject slime, float delay = 0)
    {
        StartCoroutine(InstantiateSlimeDelayed(slime, delay));
    }

    public IEnumerator InstantiateSlimeDelayed(GameObject slime, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        if (slimeInstantiated == null)
            slimeInstantiated = Instantiate(slime, launchPoint.position, Quaternion.identity).GetComponent<Slime>();
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
