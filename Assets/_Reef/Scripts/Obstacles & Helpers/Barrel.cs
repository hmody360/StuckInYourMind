using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] private GameObject _barrel;
    [SerializeField] private GameObject _brokenBarrel;
    [SerializeField] private GameObject _prefabToSpawn;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    //[SerializeField] private GameObject brokenVersion; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Impact")) {
            _barrel.SetActive(false);
            _brokenBarrel.SetActive(true);
            _audioSource.PlayOneShot(_audioSource.clip);
            if(_prefabToSpawn != null)
            {
                Instantiate(_prefabToSpawn, _brokenBarrel.transform);
            }
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.relativeVelocity.magnitude > 3f)
    //    {
    //        Instantiate(brokenVersion, transform.position, transform.rotation);
    //        Destroy(gameObject);
    //    }
    //}

}
