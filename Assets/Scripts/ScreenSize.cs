using UnityEngine;
public class ScreenSize
{

    public static float GetScreenToWorldHeight
    {
        get
        {
            return GetEdgeVector().y / 2;
        }
    }
    public static float GetScreenToWorldWidth
    {
        get
        {
            return GetEdgeVector().x;
        }
    }

    static Vector2 GetEdgeVector()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 1));
    }
}