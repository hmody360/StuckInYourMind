using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector3 _lastCheckpoint;
    private Transform _player;
    private PlayerHealth _playerHealth;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerHealth = _player.GetComponent<PlayerHealth>();
        _lastCheckpoint = _player.position;
    }

    public void SetCheckpoint(Vector3 position)
    {
        _lastCheckpoint = position;
    }

    public void RespawnPlayer()
    {
        Rigidbody rb = _player.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        _player.position = _lastCheckpoint;
    }

}
