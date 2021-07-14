using System.Collections.Generic;
using UnityEngine;

public class SplatEffectHumans : MonoBehaviour
{
    [SerializeField] private List<Material> splattedMaterial;

    [Space(5)]
    [SerializeField] private SkinnedMeshRenderer humanRenderer;    

    private bool alreadySplatted;
    private int splatIndex;
    [SerializeField] private Material[] materialsList;

    private void Awake() => materialsList = humanRenderer.materials;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Slime") && !alreadySplatted)
        {
            alreadySplatted = true;

            // Check the slime type to get the correct color to splatt effect
            Slime slime = collision.gameObject.GetComponent<Slime>();

            if (slime is SlimeCollector)
            {
                splatIndex = 0;
            }

            else if (slime is SlimeTactical)
            {
                splatIndex = 1;
            }

            else if (slime is SlimeBomb)
            {
                splatIndex = 2;
            }

            materialsList[0] = splattedMaterial[splatIndex];
            humanRenderer.materials = materialsList;
            //humanRenderer.material.SetTextureOffset("_MainTex", new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)));
        }
    }
}
