using System.Collections;
using UnityEngine;

public class HumanTacticalSlimeAttraction : MonoBehaviour
{
    public float attractionForce = 10f;
    [ReadOnly]
    [SerializeField]
    private SlimeTactical tacticalSlime;
    [ReadOnly]
    [SerializeField]
    private bool isBeingAttractd = false;
    public Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(isBeingAttractd && tacticalSlime != null)
        {
            if (Vector3.Distance(tacticalSlime.transform.position, transform.position) > 1)
            {
                ForceTowardsSlime();
            }
        }
    }

    private void ForceTowardsSlime()
    {
        Vector3 direction = tacticalSlime.transform.position - transform.position;
        rb.AddForce(Vector3.Scale(direction, new Vector3(1, 0, 1)) * attractionForce * Time.smoothDeltaTime);
    }

    public void SetAtraction(SlimeTactical slime)
    {
        if (isBeingAttractd) return;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        tacticalSlime = slime;
        isBeingAttractd = true;
    }

    public void ClearAtraction()
    {
        tacticalSlime = null;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        isBeingAttractd = false;
    }
}
