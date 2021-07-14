using UnityEngine;

public class HumanKiller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other != null)
        {
            if (other.gameObject.CompareTag("Human"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}
