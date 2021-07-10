using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static int SortRandomArrayIndex(int maxInclusive)
    {
        return Random.Range(0, maxInclusive);
    }

    public static Color32 SortRandomColorFromColors(Color32[] colors)
    {
        return colors[SortRandomArrayIndex(colors.Length)];
    }

    public static float GetRandomFloatBetween(float minInclusive, float maxInclusive)
    {
        return Random.Range(minInclusive, maxInclusive);
    }

    public static float GetRandomFloatFromVector(Vector2 bounds)
    {
        return GetRandomFloatBetween(bounds.x, bounds.y);
    }

    public static Vector3 GetRandomVectorFromBounds(Vector2 bounds)
    {
        return new Vector3(
            GetRandomFloatBetween(bounds.x, bounds.y),
            GetRandomFloatBetween(bounds.x, bounds.y),
            GetRandomFloatBetween(bounds.x, bounds.y)
        );
    }

    public static Vector3 GetRandomVectorFromBounds(Vector2 xBounds, Vector2 yBounds, Vector2 zBounds)
    {
        return new Vector3(
            GetRandomFloatBetween(xBounds.x, xBounds.y),
            GetRandomFloatBetween(yBounds.x, yBounds.y),
            GetRandomFloatBetween(zBounds.x, zBounds.y)
        );
    }
}

public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

}
