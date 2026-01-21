using UnityEngine;

public class CollectibleObj : MonoBehaviour
{
    private Renderer _renderer;
    private Collider _collider;
    [SerializeField] private ParticleSystem _collectParticle;
    [SerializeField] public Collectible _item;


    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_collider  != null && other.CompareTag("Player"))
        {
            _renderer.enabled = false;
            _collider.enabled = false;
            _collectParticle.Play();
            other.GetComponent<PlayerInventory>().AddCollectible(_item);
            Destroy(gameObject, 1f);
        }
    }
}
