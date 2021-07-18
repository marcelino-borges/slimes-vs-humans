using UnityEngine;

/// <summary>
/// Drop this class into an empty game object 
/// under which all level assets will be.
/// Btw, make the terrain prefab child of this empty game object too
/// </summary>
public class TerrainRotation : MonoBehaviour
{
    public static TerrainRotation instance;

    public float speed = 2f;

    public bool toRight = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        
    }
        
    void Update()
    {
        Vector3 axis = toRight ? Vector3.up : Vector3.down;
        transform.Rotate(axis * speed * Time.deltaTime);
    }
}
