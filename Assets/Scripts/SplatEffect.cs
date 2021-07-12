using UnityEngine;

public class SplatEffect : MonoBehaviour
{
    public bool decalDone;

    [SerializeField] private GameObject splatDecal;

    private void Splat(Vector3 transform)
    {
        //Ray ray = Camera.main.ScreenPointToRay();
        //RaycastHit hitInfo;

        //if (Physics.Raycast(ray, out hitInfo, 10f))
        //{
        //    Instantiate(splatDecal, transform, Quaternion.LookRotation(.normal));
        //}

        //Instantiate(splatDecal, collision.contacts[0].point, collision.contacts[0].normal);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (!collision.gameObject.CompareTag("Slime"))
        //{
        //    print("Points colliding: " + collision.contacts.Length);
        //}

        //Debug.Log("colidiu");
        //print("Points colliding: " + collision.contacts.Length);
        //print("First point that collided: " + collision.contacts[0].point);

        if (collision.gameObject.CompareTag("Slime") && !decalDone)
        {
            print("First point that collided: " + collision.contacts[0].normal);
            var contactPoint = collision.contacts[0];
            Instantiate(splatDecal, collision.contacts[0].point, Quaternion.FromToRotation(Vector3.up, contactPoint.normal));
            decalDone = true;
        }
    }
}
