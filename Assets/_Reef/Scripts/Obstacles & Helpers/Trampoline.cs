using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float bounceForce = 15f;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    0f,
                    rb.linearVelocity.z
                );

                rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
            }

            if(_audioSource != null)
            {
                _audioSource.PlayOneShot(_audioSource.clip);
            }
        }
    }
}
