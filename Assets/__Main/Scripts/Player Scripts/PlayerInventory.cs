using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static GameEnums;
using System;

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

    public static event Action OnMainCollectiblesCollected;

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
        GameUIManager.OnGamePaused += setIsOpenFalse;
    }

    private void OnDisable()
    {
        _input.OnInventory -= ToggleInventory;
        GameUIManager.OnGamePaused -= setIsOpenFalse;
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
        CheckAllMainCollectiblesCollected();

        if (item.getIsWearble())
        {
            if (_WearbleObject != null)
            {
                _WearbleObject.SetActive(true);
                if (_pMovement != null && _pHealth != null)
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

    public bool CheckAllMainCollectiblesCollected()
    {
        if(itemlist.Count < 3)
        {
            return false;
        }

        int count = 0;
        foreach (Collectible item in itemlist)
        {
            if (item.getType() == CollectibleType.NormalCollectible)
            {
                count++;
            }
        }

        if(count >= 3)
        {
            Debug.Log("All Main Collectibles Collected");
            OnMainCollectiblesCollected?.Invoke();  
            return true;
        }
        else
        {
            Debug.Log("All Main Collectibles Not Yet Collected");
            return false;
        }
        
    }

    public bool getIsOpen()
    {
        return isOpen;
    }

    public void setIsOpenFalse()
    {
        isOpen = false;
    }
}
