using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Color32[] destroyableBrickColors = new Color32[]
    {
        new Color32(59,177,219,86),
        new Color32(205,103,219,86),
        new Color32(81,182,219,86),
        new Color32(219,142,59,86),
        new Color32(146,219,70,86)
    };
    public static Color32[] enemyBrickColors = new Color32[]
    {
        new Color32(156,22,12,61),
        new Color32(92,13,7,36),
        new Color32(219,31,18,86),
        new Color32(232,33,19,91),
        new Color32(194,27,16,76)
    };

    public static int SortRandomArrayIndex(int maxInclusive)
    {
        return Random.Range(0, maxInclusive);
    }

    public static Color32 SortRandomColorFromColors(Color32[] colors)
    {
        return colors[SortRandomArrayIndex(colors.Length)];
    }
}
