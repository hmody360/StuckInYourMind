using TMPro;
using UnityEngine;
using static GameEnums;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject _backgroundOverlay;

    [Header("Inventory Related")]
    // Main Inventory
    [SerializeField] private TextMeshProUGUI _backpackTipText;
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private List<GameObject> _itemSlotList;
    [SerializeField] private GameObject _secretCollectibleTextObj;
    // Inventory Item Viewer
    [SerializeField] private GameObject _itemViewerPanel;
    [SerializeField] private Image _selectedItemIcon;
    [SerializeField] private TextMeshProUGUI _selectedItemName;
    [SerializeField] private TextMeshProUGUI _selectedItemDescription;

    public static GameUIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _backgroundOverlay.SetActive(false);
        _inventoryPanel.SetActive(false);
    }

    // 
    public void UpdateBackpackTipText(string text)
    {
        _backpackTipText.text = text;
    }

    public void ToggleInventoryPanel()
    {
        _inventoryPanel.SetActive(!_inventoryPanel.activeSelf);
        ToggleBackgroundOverlay();
        CloseItemViewer();
    }

    public void AddItem(Collectible item)
    {
        if (_itemSlotList != null)
        {
            if (item.getType() == CollectibleType.NormalCollectible)
            {
                GameObject slotToFill = _itemSlotList.Find(obj => obj.GetComponent<Button>().interactable == false);
                FillItemSlotInfo(slotToFill, item);

            }
            else
            {
                GameObject slotToFill = _itemSlotList[_itemSlotList.Count - 1];
                FillItemSlotInfo(slotToFill, item);
            }
        }
        else
        {
            Debug.LogWarning("ItemSlot Not Set Up");
        }


    }

    public void SetItemInfo(Collectible item)
    {
        _selectedItemIcon.sprite = item.getIcon();
        _selectedItemName.text = item.getName();
        _selectedItemDescription.text = item.getDesc();
    }

    public void OpenItemViewer()
    {
        if (_itemViewerPanel != null)
            _itemViewerPanel.SetActive(true);
        _secretCollectibleTextObj.SetActive(false);
    }

    public void CloseItemViewer()
    {
        if (_itemViewerPanel != null)
            _itemViewerPanel.SetActive(false);
        _secretCollectibleTextObj.SetActive(true);
    }

    private void FillItemSlotInfo(GameObject slot, Collectible item)
    {
        slot.GetComponent<Button>().interactable = true;
        slot.GetComponentsInChildren<Image>()[1].sprite = item.getIcon();
        slot.GetComponent<InventorySlot>().SetItem(item);
    }

    private void ToggleBackgroundOverlay()
    {
        _backgroundOverlay.SetActive(!_backgroundOverlay.activeSelf);
    }
}
