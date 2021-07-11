using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] protected GameObject explosionParticlesPrefab;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explode()
    {
        if(explosionParticlesPrefab != null) {
            Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
