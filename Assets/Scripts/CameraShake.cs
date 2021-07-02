using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCo(duration, magnitude));
    }

    private IEnumerator ShakeCo(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            float x = SortRandomMagnitude(magnitude);
            float y = SortRandomMagnitude(magnitude);

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    private static float SortRandomMagnitude(float magnitude)
    {
        return Random.Range(-1f, 1f) * magnitude;
    }
}
