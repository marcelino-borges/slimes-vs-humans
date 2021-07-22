using UnityEngine;

public class SlimeBlueMagneticVFX : MonoBehaviour
{
    [SerializeField] private Slime slime;

    private void Awake() => transform.localScale = new Vector3(slime.DecayRadius, slime.DecayRadius, slime.DecayRadius) * 2f;
}
