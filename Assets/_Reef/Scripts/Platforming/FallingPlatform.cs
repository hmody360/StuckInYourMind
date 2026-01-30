using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : PlatformBase
{
    [Header("Shake Settings")]
    public float shakeDuration = 1f;
    public float shakeStrength = 0.05f;

    [Header("Fall Settings")]
    public float fallDelay = 0.2f;
    public float resetDelay = 3f;

    private Rigidbody rb;
    private AudioSource _audioSource;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool triggered = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();

        rb.isKinematic = true;
        rb.useGravity = false;

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (triggered) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            triggered = true;
            if(_audioSource != null)
            {
                _audioSource.PlayOneShot(_audioSource.clip);
            }
            StartCoroutine(FallSequence());
        }
    }

    IEnumerator FallSequence()
    {
        // 1️⃣ اهتزاز
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector3 offset = Random.insideUnitSphere * shakeStrength;
            transform.position = startPosition + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;

        
        yield return new WaitForSeconds(fallDelay);

     
        rb.isKinematic = false;
        rb.useGravity = true;

        //--------------------------
        yield return new WaitForSeconds(resetDelay);

        ResetPlatform();
    }

    void ResetPlatform()
    {
        rb.isKinematic = true;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = startPosition;
        transform.rotation = startRotation;

        triggered = false;
    }
}
