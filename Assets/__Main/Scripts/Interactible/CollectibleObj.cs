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
            
            switch (_item.getType())
            {
                case CollectibleType.NormalCollectible:
                    _playerInventory.AddCollectible(_item);
                    GameUIManager.instance.AddItem(_item);
                    DestroyCollectible();
                    break;
                case CollectibleType.SecretCollectible:
                    _playerInventory.AddCollectible(_item);
                    GameManager.instance.AddSecretCollectible();
                    GameUIManager.instance.AddItem(_item);
                    DestroyCollectible();
                    break;
                case CollectibleType.HealthPoint:
                    bool canAddHealth = _playerHealth.AddHealthPoint();
                    if (canAddHealth)
                        DestroyCollectible();
                    // Update Health UI
                    break;
                case CollectibleType.LifePoint:
                    bool canAddLife = _playerHealth.AddLifePoint();
                    if (canAddLife)
                        DestroyCollectible();
                    // Update Lives in UI & Show Lives UI for a bit then disapper
                    break;


            }
        }
    }

    private void DestroyCollectible()
    {
        FadeOut();
        _collider.enabled = false;
        _collectParticle.Play();
        Destroy(gameObject, 1f);
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
