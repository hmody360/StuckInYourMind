using System.Collections;
using UnityEngine;
using static GameEnums;

public class CollectibleObj : MonoBehaviour
{
    private Renderer _renderer;
    private Collider _collider;
    [SerializeField] private ParticleSystem _collectParticle;
    [SerializeField] public Collectible _item;
    [SerializeField] private float _fadeOutSpeed = 2f;
    private Vector3 _initialScale;
    private PlayerInventory _playerInventory;
    private PlayerHealth _playerHealth;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
        _initialScale = transform.localScale;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerInventory = player.GetComponent<PlayerInventory>();
        _playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_collider != null && other.CompareTag("Player"))
        {
            FadeOut();
            _collider.enabled = false;
            _collectParticle.Play();
            Destroy(gameObject, 1f);

            switch (_item.getType())
            {
                case CollectibleType.NormalCollectible:
                    _playerInventory.AddCollectible(_item);
                    // Add To Normal Item Spot in UI
                    break;
                case CollectibleType.SecretCollectible:
                    _playerInventory.AddCollectible(_item);
                    // Add To Secret Item Spot in UI
                    break;
                case CollectibleType.HealthPoint:
                    _playerHealth.AddHealthPoint();
                    // Update Health UI
                    break;
                case CollectibleType.LifePoint:
                    _playerHealth.AddLifePoint();
                    // Update Lives in UI & Show Lives UI for a bit then disapper
                    break;


            }
        }
    }

    private void FadeOut()
    {
        StartCoroutine(FadeOutObject());
    }

    private IEnumerator FadeOutObject()
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * _fadeOutSpeed;
            transform.localScale = Vector3.Lerp(_initialScale, Vector3.zero, t);
            yield return null;
        }
    }

}
