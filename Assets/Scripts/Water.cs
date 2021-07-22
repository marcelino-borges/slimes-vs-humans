using UnityEngine;

public class Water : MonoBehaviour
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
        if (collision != null)
        {
            if(collision.gameObject.CompareTag("Slime"))
                Destroy(collision.gameObject);

            if (collision.gameObject.CompareTag("Human"))
                LevelManager.instance.SetGameOver();
        }
    }
}
