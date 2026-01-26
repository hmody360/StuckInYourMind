using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<Collectible> itemlist = new List<Collectible>();
    [SerializeField] private AudioSource _inventoryAudioSource;
    [SerializeField] private AudioClip[] _inventoryAudioClips;
    [SerializeField] private GameObject _WearbleObject;
    private bool isOpen = false;
    private Animator _animator;
    private PlayerInputHandler _input;
    private PlayerMovement _pMovement;
    private PlayerHealth _pHealth;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _input = GetComponent<PlayerInputHandler>();
        _pMovement = GetComponent<PlayerMovement>();
        _pHealth = GetComponent<PlayerHealth>();
    }


    private void OnEnable()
    {
        _input.OnInventory += ToggleInventory;
    }

    private void OnDisable()
    {
        _input.OnInventory -= ToggleInventory;
    }

    public void ToggleInventory()
    {
        GameUIManager.instance.ToggleInventoryPanel();

        if (isOpen)
        {
            _inventoryAudioSource.PlayOneShot(_inventoryAudioClips[0]);
            GameUIManager.instance.UpdateBackpackTipText("Open Backpack (E)");
            GameManager.instance.ResumeGame();
        }
        else
        {
            _inventoryAudioSource.PlayOneShot(_inventoryAudioClips[1]);
            GameUIManager.instance.UpdateBackpackTipText("Close Backpack (E)");
            GameManager.instance.PauseGame();
        }

        isOpen = isOpen ? false : true;
    }

    public void AddCollectible(Collectible item)
    {
        itemlist.Add(item);
        _animator.SetTrigger("PickupTrigger");
        _inventoryAudioSource.PlayOneShot(_inventoryAudioClips[2]);

        if (item.getIsWearble())
        {
            if(_WearbleObject != null)
            {
                _WearbleObject.SetActive(true);
                if(_pMovement != null && _pHealth != null)
                {
                    _pMovement.UpdateRendererList();
                    _pHealth.UpdateRendererList();
                }
                else
                {
                    Debug.LogWarning("Player movement or player health is null");
                }
            }
            else
            {
                Debug.LogWarning("Wearble Not assigned");
            }
            
        }
    }
}
