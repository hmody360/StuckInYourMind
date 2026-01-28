using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxHealth;

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            other.collider.GetComponent<PlayerHealth>().takeDamage(1);
        }
    }
}
