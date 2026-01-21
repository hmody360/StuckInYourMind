using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<Collectible> itemlist = new List<Collectible>();
    [SerializeField] private AudioSource _inventoryAudioSource;
    [SerializeField] private AudioClip[] _inventoryAudioClips;
    private bool isOpen = false;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) //Open/Close Inventory UI and play the respective sound
        {
            //UIManger.instance.ToggleCollectibles();

            if (isOpen)
            {
                _inventoryAudioSource.PlayOneShot(_inventoryAudioClips[0]);
            }
            else
            {
                _inventoryAudioSource.PlayOneShot(_inventoryAudioClips[1]);
            }

            isOpen = isOpen ? false : true;
        }
    }

    public void AddCollectible(Collectible item)
    {
        itemlist.Add(item);
        _animator.SetTrigger("PickupTrigger");
        _inventoryAudioSource.PlayOneShot(_inventoryAudioClips[2]);
    }
}
