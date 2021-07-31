using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.CompareTag(GameManager.SLIME_TAG))
        {
            Destroy(other.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag(GameManager.SLIME_TAG))
                collision.gameObject.GetComponent<Slime>().Die(false);

            if (collision.gameObject.CompareTag(GameManager.HUMAN_TAG))
                LevelManager.instance.SetVictory();
        }
    }
}
