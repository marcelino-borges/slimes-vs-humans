using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Building : MonoBehaviour
{
    public bool spawnRandomBuilding = false;

    [Header("EXPLOSION DETAILS")]
    [MinMaxSlider(600f,3000f)]
    public Vector2 explosionForce = Vector2.one;
    public float explosionRadius = 1f;
    public float explosionUpwardsModifier = 1f;
    public ForceMode explosionForceMode;
    public ParticleSystem explosionParticles;
    public float explosionPossiblePositionsVolumeSize = 2f;
    private Vector3 explosionPosition;

    [Header("EXTERNAL REFERENCES")]
    [Tooltip("Se spawnRandomBuilding estiver como false, será instanciado o primeito prefab do array, se estiver como true, instanciará um prefab aleatório do array.")]
    public GameObject[] buildingsPrefabs;

    [Header("SOUND EFFECTS")]
    public AudioClip[] explosionSfx;

    private AudioSource audioSource;
    private GameObject buildingObject;
    private Rigidbody rb;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        audioSource.volume = SoundManager.instance.CurrentVolume;
        InstantiateRandomBuilding();
    }

    private void InstantiateRandomBuilding()
    {
        GameObject prefabToInstantiate = null;

        if (buildingsPrefabs != null && buildingsPrefabs.Length > 0)
        {
            if (spawnRandomBuilding)
                prefabToInstantiate = buildingsPrefabs[Random.Range(0, buildingsPrefabs.Length)];
            else
                prefabToInstantiate = buildingsPrefabs[0];
        }

        if (prefabToInstantiate != null)
            buildingObject = Instantiate(prefabToInstantiate, transform.position, prefabToInstantiate.transform.rotation, transform);

        if (buildingObject != null)
            rb = buildingObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Explode();
        }
#endif
    }

    public void Explode(float delay = 0f)
    {
        StartCoroutine(ExplodeCo(delay));
    }

    private IEnumerator ExplodeCo(float delay)
    {
        yield return new WaitForSeconds(delay);
        explosionPosition = new Vector3(
            Utils.GetRandomFloatBetween(-explosionPossiblePositionsVolumeSize/2, explosionPossiblePositionsVolumeSize / 2),
            transform.position.y,
            Utils.GetRandomFloatBetween(-explosionPossiblePositionsVolumeSize / 2, explosionPossiblePositionsVolumeSize / 2)
        );
        rb.AddExplosionForce(Utils.GetRandomFloatFromVector(explosionForce), transform.position + explosionPosition, explosionRadius, explosionUpwardsModifier, explosionForceMode);

        PlayExplosionParticles();

        if (explosionSfx != null && explosionSfx.Length > 0)
            PlaySfx(GetRandomExplosionClip());

        Destroy(gameObject, 2f);
    }

    private void PlaySfx(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomExplosionClip()
    {        
        return explosionSfx[Random.Range(0, explosionSfx.Length)];
    }

    private void PlayExplosionParticles()
    {
        if (explosionParticles != null)
        {
            explosionParticles.Play();
            Destroy(explosionParticles.gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(explosionPossiblePositionsVolumeSize, 0f, explosionPossiblePositionsVolumeSize));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
#endif
}
