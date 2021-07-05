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
}
