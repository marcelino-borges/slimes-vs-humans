using UnityEngine;

public class HumanKiller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other != null)
        {
            if (other.gameObject.CompareTag(GameManager.HUMAN_TAG))
            {
                Destroy(other.gameObject);
            }
        }
    }
}
