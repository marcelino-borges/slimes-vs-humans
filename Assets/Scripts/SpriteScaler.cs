using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScaler : MonoBehaviour
{
    void Start()
    {
        float width = ScreenSize.GetScreenToWorldWidth;
        float height= ScreenSize.GetScreenToWorldHeight;
        transform.localScale = new Vector3(width, height, 1);
    }
}
