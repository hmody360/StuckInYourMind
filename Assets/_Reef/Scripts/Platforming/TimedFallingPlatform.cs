using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class TimedFallingPlatform : PlatformBase
{
    [Header("Timing")]
    public float activeTime = 3f;        
    public float shakeDuration = 0.6f;   
    public float shakeStrength = 0.05f;  

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Coroutine routine;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;

        startPosition = transform.position;
        startRotation = transform.rotation;

        gameObject.SetActive(false);
    }

    public override void Activate()
    {
        if (routine != null)
            StopCoroutine(routine);

        gameObject.SetActive(true);
        routine = StartCoroutine(FallSequence());
    }

    IEnumerator FallSequence()
    {
        
        yield return new WaitForSeconds(activeTime);

     
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector3 offset = Random.insideUnitSphere * shakeStrength;
            transform.position = startPosition + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;

   
        rb.isKinematic = false;
        rb.useGravity = true;

        routine = null;
    }


    public void ResetPlatform()
    {
        StopAllCoroutines();

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = startPosition;
        transform.rotation = startRotation;

        gameObject.SetActive(false);
    }
}
