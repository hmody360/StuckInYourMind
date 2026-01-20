using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        CheckpointManager.Instance.RespawnPlayer();
    }
}
