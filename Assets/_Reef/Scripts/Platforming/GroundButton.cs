using UnityEngine;
using UnityEngine.Events;

public class GroundButton : MonoBehaviour
{
    [Header("Button Settings")]
    public UnityEvent OnPressed;


    private ParticleSystem _particleSystem;
    private AudioSource _audioSource;
    private bool pressed = false;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (pressed) return;

        if (other.CompareTag("Player"))
        {
            pressed = true;
            _particleSystem.Stop();
            _audioSource.PlayOneShot(_audioSource.clip);
            OnPressed.Invoke();
        }
    }
}
