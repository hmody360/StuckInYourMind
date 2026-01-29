using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private AudioSource _audiosource;
    [SerializeField] private ParticleSystem _CheckpointReachedVFX;

    private void Awake()
    {
        _audiosource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        PlayerHealth.OnCheckPointSet += CheckCurrentCheckPoint;
    }

    private void OnDisable()
    {
        PlayerHealth.OnCheckPointSet -= CheckCurrentCheckPoint;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if(other.GetComponent<PlayerHealth>().GetCheckPoint() == transform.position)
            {
                return;
            }
            other.GetComponent<PlayerHealth>().SetCheckpoint(transform.position);
            
        }
    }

    private void CheckCurrentCheckPoint(Vector3 checkpoint)
    {
        if(checkpoint == transform.position)
        {
            GameUIManager.instance.ShowPromptTimed(2f, "CheckPoint Reached!");
            _audiosource.PlayOneShot(_audiosource.clip);
            if (_CheckpointReachedVFX != null)
            {
                _CheckpointReachedVFX.Play();
            }
        }
        else
        {
            _CheckpointReachedVFX.Stop();
        }
    }
}
