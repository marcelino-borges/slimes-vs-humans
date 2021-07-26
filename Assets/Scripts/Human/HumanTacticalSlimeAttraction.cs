using UnityEngine;

public class HumanTacticalSlimeAttraction : MonoBehaviour
{
    public float attractionForce = 10f;
    [ReadOnly]
    [SerializeField]
    private SlimeTactical tacticalSlime;
    [ReadOnly]
    [SerializeField]
    private bool isBeingAttracted = false;
    public Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(tacticalSlime != null && tacticalSlime.gameObject.activeInHierarchy)
        {

            if (isBeingAttracted && Vector3.Distance(tacticalSlime.transform.position, transform.position) > 1)
            {
                ForceTowardsSlime();
            }
        } else
        {
            ClearAtraction();
        }
    }

    private void ForceTowardsSlime()
    {
        Vector3 direction = tacticalSlime.transform.position - transform.position;
        rb.AddForce(Vector3.Scale(direction, new Vector3(1, 0, 1)) * attractionForce * Time.smoothDeltaTime);
    }

    public void SetAtraction(SlimeTactical slime)
    {
        if (isBeingAttracted) return;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        tacticalSlime = slime;
        isBeingAttracted = true;
    }

    public void ClearAtraction()
    {
        tacticalSlime = null;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        isBeingAttracted = false;
    }
}
