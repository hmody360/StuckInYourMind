using UnityEngine;

public class AttackImpact : MonoBehaviour
{
    public float attackPower;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"Attack inflicted {attackPower} Damage");
            Destroy(other.gameObject);
        }
    }
}
