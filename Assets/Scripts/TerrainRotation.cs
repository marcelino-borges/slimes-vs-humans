using UnityEngine;

/// <summary>
/// Drop this class into an empty game object 
/// under which all level assets will be.
/// Btw, make the terrain prefab child of this empty game object too
/// </summary>
public class TerrainRotation : MonoBehaviour
{
    [ReadOnly]
    [SerializeField]
    [Tooltip("Set the rotation speed in the LevelManager of this scene")]
    private float _speed = 2f;
    [ReadOnly]
    [SerializeField]
    private bool isRotating = true;

    public static TerrainRotation instance;

    public bool toRight = false;
    public bool canRotate = true;

    public float Speed { get => _speed; set => _speed = value; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        _speed = LevelManager.instance.terrainRotationSpeed;
    }
        
    void Update()
    {
        if (canRotate && isRotating)
        {
            Vector3 axis = toRight ? Vector3.up : Vector3.down;
            transform.Rotate(axis * _speed * Time.deltaTime);
        }
    }

    public void Rotate()
    {
        isRotating = true;
    }

    public void Stop()
    {
        isRotating = false;
    }

    public void InvertRotation()
    {
        toRight = !toRight;
    }
}
