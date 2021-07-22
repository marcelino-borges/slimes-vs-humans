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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Slime"))
        {
            Destroy(collision.gameObject);
        }
    }
}
