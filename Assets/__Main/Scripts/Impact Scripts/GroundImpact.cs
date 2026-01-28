using UnityEngine;

public class GroundImpact : MonoBehaviour
{
    [SerializeField] private float impactTime;
    [SerializeField] private float impactForce = 3f;

    private void Start()
    {
        Destroy(gameObject, impactTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyAI>().takeDamage(impactForce);
        }
    }
}
