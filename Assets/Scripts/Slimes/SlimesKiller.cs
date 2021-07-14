using UnityEngine;

public class SlimesKiller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.CompareTag("Slime"))
        {
            Destroy(other.gameObject);
        }
    }
}
