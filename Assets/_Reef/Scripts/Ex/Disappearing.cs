using System.Collections;
using UnityEngine;

public class Disappearing : MonoBehaviour
{
    public float respawnTime = 3f;

    Collider col;
    Renderer mesh;
    bool used = false;

    void Awake()
    {
        col = GetComponent<Collider>();
        mesh = GetComponent<Renderer>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !used)
        {
            used = true;
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
