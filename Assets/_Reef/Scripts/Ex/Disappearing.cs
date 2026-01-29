using System.Collections;
using UnityEngine;

public class Disappearing : MonoBehaviour
{
    public float respawnTime = 3f;

    Collider col;
    Renderer mesh;
    bool used = false;

    private AudioSource _audioSource;

    void Awake()
    {
        col = GetComponent<Collider>();
        mesh = GetComponent<Renderer>();
        _audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !used)
        {
            used = true;
            if(_audioSource != null)
            {
                _audioSource.PlayOneShot(_audioSource.clip);
            }
            StartCoroutine(DisappearAndReturn());
        }
    }

    IEnumerator DisappearAndReturn()
    {
        // تختفي فورًا
        mesh.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        mesh.enabled = true;
        col.enabled = true;
        used = false;
    }
}
