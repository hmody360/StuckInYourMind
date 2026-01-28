using UnityEngine;

public class AttackImpact : MonoBehaviour
{
    public float attackPower;
    private AudioSource _punchLandAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _punchLandAudioSource = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"Attack inflicted {attackPower} Damage");
            if(_punchLandAudioSource != null)
            {
                _punchLandAudioSource.PlayOneShot(_punchLandAudioSource.clip);
            }
            other.GetComponent<EnemyAI>().takeDamage(attackPower);
        }
    }
}
