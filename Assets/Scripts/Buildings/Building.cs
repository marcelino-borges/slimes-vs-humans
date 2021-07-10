﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Building : MonoBehaviour
{
    public bool spawnRandomBuilding = false;

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
    [Header("EXTERNAL REFERENCES")]
    [Tooltip("Se spawnRandomBuilding estiver como false, será instanciado o primeito prefab do array, se estiver como true, instanciará um prefab aleatório do array.")]
    [SerializeField] private GameObject[] _buildingsPrefabs;
    [Header("SOUND EFFECTS")]
    [SerializeField] private AudioClip[] _explosionSfx;
    private AudioSource _audioSource;
    private GameObject _buildingObject;
    private Rigidbody _rb;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        _audioSource.volume = SoundManager.instance.CurrentVolume;
        InstantiateRandomBuilding();
    }

    private void InstantiateRandomBuilding()
    {
        GameObject prefabToInstantiate = null;

        if (_buildingsPrefabs != null && _buildingsPrefabs.Length > 0)
        {
            if (spawnRandomBuilding)
                prefabToInstantiate = _buildingsPrefabs[Random.Range(0, _buildingsPrefabs.Length)];
            else
                prefabToInstantiate = _buildingsPrefabs[0];
        }

        if (prefabToInstantiate != null)
            _buildingObject = Instantiate(prefabToInstantiate, transform.position, prefabToInstantiate.transform.rotation, transform);

        if (_buildingObject != null)
            _rb = _buildingObject.GetComponent<Rigidbody>();
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
        gameObject.isStatic = false;
        _rb.isKinematic = false;
        Vector3 explosionPosition = new Vector3(
            Utils.GetRandomFloatBetween(-_explosionPossiblePositionsVolumeSize/2, _explosionPossiblePositionsVolumeSize / 2),
            transform.position.y,
            Utils.GetRandomFloatBetween(-_explosionPossiblePositionsVolumeSize / 2, _explosionPossiblePositionsVolumeSize / 2)
        );
        _rb.AddExplosionForce(Utils.GetRandomFloatFromVector(_explosionForce), transform.position + explosionPosition, _explosionRadius, _explosionUpwardsModifier, _explosionForceMode);
        _rb.angularVelocity = Utils.GetRandomVectorFromBounds(_explosionAngularRotation);
        PlayExplosionParticles();

        if (_explosionSfx != null && _explosionSfx.Length > 0)
            PlaySfx(GetRandomExplosionClip());

        Destroy(gameObject, 1f);
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
