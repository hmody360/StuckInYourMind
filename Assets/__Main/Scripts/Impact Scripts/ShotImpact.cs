using UnityEngine;

public class ShotImpact : MonoBehaviour
{
    public float shotDamage;
    public float shotTime;

    private void Start()
    {
        Destroy(gameObject, shotTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"Shot inflicted {shotDamage} Damage");
            other.GetComponent<EnemyAI>().takeDamage(shotDamage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


}
