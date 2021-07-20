using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Building : MonoBehaviour
{
    private enum BuildingType {
        None,
        Random,
        House1,
        House2,
        House3,
        House4,
        Skyline1,
        Skyline2, 
        CityHall
    }

    private AudioSource _audioSource;
    private GameObject _buildingObject;
    [Tooltip("If set to \"None\", no building is spawned. If \"Random\", a random building is spawned.")]
    [SerializeField] private BuildingType _type = BuildingType.Random;
    [Header("EXPLOSION DETAILS")]
    [MinMaxSlider(1f,3000f)]
    [SerializeField] private Vector2 _explosionForce = Vector2.one;
    [Tooltip("Angular velocity after the explosion (rad/s).")]
    [MinMaxSlider(-3000f, 3000f)]
    [SerializeField] private Vector2 _explosionAngularRotation = Vector2.one;
    [MinMaxSlider(0.5f, 3)]
    [SerializeField] private Vector2 _lifeSpanAfterExplosion = new Vector2(0.5f, 3f);
    [SerializeField] private float _explosionRadius = 1f;
    [SerializeField] private float _explosionUpwardsModifier = 1f;
    [SerializeField] private ForceMode _explosionForceMode;
    [SerializeField] private ParticleSystem _explosionParticles;
    [SerializeField] private float _explosionPossiblePositionsVolumeSize = 2f;
    [Tooltip("Se spawnRandomBuilding estiver como false, será instanciado o primeito prefab do array, se estiver como true, instanciará um prefab aleatório do array.")]
    [SerializeField] private GameObject[] _buildingsPrefabs;
    [Header("SOUND EFFECTS")]
    [SerializeField] private AudioClip[] _explosionSfx;
    [SerializeField] private Rigidbody _rb; //Serializado
    [SerializeField] private Transform[] humanPositions;
    [SerializeField] private int _humansToSpawnOnRoof = 0;
    [SerializeField] private List<Human> _humansOnRooftop;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        _audioSource.volume = SoundManager.instance.CurrentVolume;
        InstantiateBuilding();
        SpawnHumansOnRooftop();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Explode();
        }
#endif
    }

    private void SpawnHumansOnRooftop()
    {
        GameObject humanPrefab = ObjectPooler.instance.GetOriginalPrefabFromPool("human");

        for (int i = 0; i < _humansToSpawnOnRoof; i++)
        {
            if (humanPositions[i] != null) {

                GameObject humanSpawned = Instantiate(
                    humanPrefab,
                    humanPositions[i].position, 
                    Quaternion.identity, 
                    transform
                );

                if(humanSpawned != null)
                {
                    Human human = humanSpawned.GetComponent<Human>();

                    if (human != null)
                    {
                        human.rb.isKinematic = true;
                        human.CanBeInfected = false;
                        _humansOnRooftop.Add(human);
                    }
                }
            }
        }
    }

    private void InstantiateBuilding()
    {
        GameObject prefabToInstantiate = null;

        if (_buildingsPrefabs != null && _buildingsPrefabs.Length > 0)
        {
            prefabToInstantiate = GetBuildingByName(_type);
        }

        if (prefabToInstantiate != null)
        {
            _buildingObject = Instantiate(prefabToInstantiate, transform.position, prefabToInstantiate.transform.rotation, transform);

            if (_buildingObject != null)
                _rb = _buildingObject.GetComponent<Rigidbody>();
        }
    }

    GameObject GetBuildingByName(BuildingType type)
    {
        GameObject buildingToSpawn = null;

        if (type != BuildingType.None)
        {
            if (type != BuildingType.Random)
            {
                foreach (GameObject building in _buildingsPrefabs)
                {
                    if (building.name.ToLower().Contains(type.ToString().ToLower()))
                        buildingToSpawn = building;
                }
            }
            else
            {
                buildingToSpawn = Utils.GetRandomArrayElement(_buildingsPrefabs);
            }
        }
        return buildingToSpawn;
    }

    public void Explode(float delay = 0f)
    {
        StartCoroutine(ExplodeCo(delay));
    }

    private IEnumerator ExplodeCo(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.isStatic = false;
        AddPhysicsExplosion();
        PlayExplosionParticles();

        if (_explosionSfx != null && _explosionSfx.Length > 0)
            PlaySfx(GetRandomExplosionClip());
        StartCoroutine(ReleaseHumans(.3f));

        Destroy(gameObject, 1f);
    }

    private IEnumerator ReleaseHumans(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_humansOnRooftop != null && _humansOnRooftop.Count > 0)
        {
            foreach (Human human in _humansOnRooftop)
            {
                human.gameObject.transform.SetParent(transform.parent);
                human.gameObject.transform.position = new Vector3(
                    transform.position.x + Random.Range(-1, 2),
                    transform.position.y,
                    transform.position.z + Random.Range(-1, 2)
                );
                human.CanBeInfected = true;
            }
        }
    }

    private void AddPhysicsExplosion()
    {
        _rb.isKinematic = false;
        Vector3 explosionPosition = new Vector3(
            Utils.GetRandomFloatBetween(-_explosionPossiblePositionsVolumeSize / 2, _explosionPossiblePositionsVolumeSize / 2),
            transform.position.y,
            Utils.GetRandomFloatBetween(-_explosionPossiblePositionsVolumeSize / 2, _explosionPossiblePositionsVolumeSize / 2)
        );
        _rb.AddExplosionForce(Utils.GetRandomFloatFromBounds(_explosionForce), transform.position + explosionPosition, _explosionRadius, _explosionUpwardsModifier, _explosionForceMode);
        _rb.angularVelocity = Utils.GetRandomVectorFromBounds(_explosionAngularRotation);
    }

    private void PlaySfx(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomExplosionClip()
    {        
        return _explosionSfx[Random.Range(0, _explosionSfx.Length)];
    }

    private void PlayExplosionParticles()
    {
        if (_explosionParticles != null)
        {
            _explosionParticles.Play();
            Destroy(_explosionParticles.gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(_explosionPossiblePositionsVolumeSize, 0f, _explosionPossiblePositionsVolumeSize));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
#endif
}
