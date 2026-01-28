using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] private GameObject barrel;
    [SerializeField] private GameObject brokenBarrel;
    [SerializeField] private KeyCode breakKey=KeyCode.P;

    //[SerializeField] private GameObject brokenVersion; 

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")&&Input.GetKeyDown(KeyCode.P)) {
            barrel.SetActive(false);
            brokenBarrel.SetActive(true);
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.relativeVelocity.magnitude > 3f)
    //    {
    //        Instantiate(brokenVersion, transform.position, transform.rotation);
    //        Destroy(gameObject);
    //    }
    //}

}
