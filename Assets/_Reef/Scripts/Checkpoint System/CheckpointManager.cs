using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector3 lastCheckpoint;
    private Transform player;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastCheckpoint = player.position;
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpoint = position;
    }

    public void RespawnPlayer()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        player.position = lastCheckpoint;
    }

}
