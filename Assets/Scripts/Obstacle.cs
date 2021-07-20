using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] protected GameObject explosionParticlesPrefab;
    
    public bool killSlime = false;

    public void Explode()
    {
        if(explosionParticlesPrefab != null) {
            Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.gameObject.CompareTag("Slime") && killSlime)
            {
                Slime slime = other.gameObject.GetComponent<Slime>();

                if (slime != null && !(slime is SlimeBomb))
                {
                    slime.Die();
                }
            }
        }
    }
}
