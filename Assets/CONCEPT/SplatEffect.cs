using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatEffect : MonoBehaviour
{
    [Header("List of splat prefab of each slimes")]
    [SerializeField] private List<GameObject> splatsDecal;
    [SerializeField] private List<Color> splatColor;

    [Space(5)]
    [Tooltip("Interaction layers for splashback ")]
    [SerializeField] private LayerMask layersToCheck;

    private bool canSplat;

    private ContactPoint slimeContactPoint;
    private GameObject selectedSplash;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Slime") && !canSplat)
        {
            Slime slime = collision.gameObject.GetComponent<Slime>();

            // Get contact point
            slimeContactPoint = collision.contacts[0];

            // Instantiate random splat object
            selectedSplash = Instantiate(splatsDecal[Random.Range(0, splatsDecal.Count)], collision.contacts[0].point, Quaternion.LookRotation(Vector3.up, slimeContactPoint.normal));
            selectedSplash.transform.SetParent(gameObject.transform, true);

            if (slime is SlimeCollector)
            {
                selectedSplash.GetComponent<Renderer>().material.SetColor("_Color", splatColor[0]);
            }

            else if (slime is SlimeTactical)
            {
                selectedSplash.GetComponent<Renderer>().material.SetColor("_Color", splatColor[1]);
            }

            else if (slime is SlimeBomb)
            {
                selectedSplash.GetComponent<Renderer>().material.SetColor("_Color", splatColor[2]);
            }

            //Instantiate(selectedSplash, collision.contacts[0].point, Quaternion.LookRotation(Vector3.up, slimeContactPoint.normal));
            canSplat = true;

            StartCoroutine(SplatEffectCD());
        }
    }

    private IEnumerator SplatEffectCD()
    {
        yield return new WaitForSeconds(0.01f);
        canSplat = false;
    }
}
