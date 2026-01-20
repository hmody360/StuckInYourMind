using UnityEngine;

public class GroundImpact : MonoBehaviour
{
    [SerializeField] private float impactTime;

    private void Start()
    {
        Destroy(gameObject, impactTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
