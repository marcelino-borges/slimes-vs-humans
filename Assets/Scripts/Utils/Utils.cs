using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static int SortRandomArrayIndex(int maxInclusive)
    {
        return UnityEngine.Random.Range(0, maxInclusive);
    }

    public static Color32 SortRandomColorFromColors(Color32[] colors)
    {
        return colors[SortRandomArrayIndex(colors.Length)];
    }

    public static float GetRandomFloatBetween(float minInclusive, float maxInclusive)
    {
        return UnityEngine.Random.Range(minInclusive, maxInclusive);
    }

    public static float GetRandomFloatFromBounds(Vector2 bounds)
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

    public static float GetVelocityNeededToReachDistance(float targetDistance, float gravity, float radianAngle)
    {
        return Mathf.Sqrt((targetDistance * gravity) / Mathf.Sin(2 * radianAngle));
    }

    public static float GetZMaxDistance(float velocity, float radianAngle, float gravity, float z0 = 0)
    {
        return z0 + (Mathf.Pow(velocity, 2) * Mathf.Sin(2 * radianAngle)) / gravity;
    }

    public static float GetYWhenAtZPosition(float zPosition, float velocity, float radianAngle, float gravity, float y0 = 0)
    {
        return y0 + (zPosition * Mathf.Tan(radianAngle)) - ((gravity * Mathf.Pow(zPosition, 2)) / (2 * (Mathf.Pow((velocity * Mathf.Cos(radianAngle)), 2))));
    }

    public static T GetRandomArrayElement<T>(T[] array)
    {
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static bool IsArrayValid<T>(T[] array)
    {
        return array != null && array.Length > 0;
    }
}

public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    public static void Shuffle<T>(this IList<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}
