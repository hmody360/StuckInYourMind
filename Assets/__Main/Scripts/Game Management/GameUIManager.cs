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
    [Header("Avatar Related")]
    // Health UI
    [SerializeField] private TextMeshProUGUI _livesCounter;
    [SerializeField] private GameObject _healthContainer;
    [SerializeField] private GameObject _heartPrefab;
    // Indicator UI
    [SerializeField] private Image _shootIndicator;
    [SerializeField] private Image _punchIndicator;
    [SerializeField] private Image _specialIndicator;
    [SerializeField] private Sprite[] _indicatorSpriteList;
    // Stamina UI
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Color _normalStaminaColor;
    [SerializeField] private Color _outOfStaminaColor;

    private List<GameObject> _heartList = new List<GameObject>();

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

    //=================== General ===================

    private void ToggleBackgroundOverlay()
    {
        _backgroundOverlay.SetActive(!_backgroundOverlay.activeSelf);
    }

    //=================== Inventory Related ===================
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

    //=================== Inventory Viewer Related ===================
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

    //=================== Player  Stats Related ===================

    // Health UI Logic
    public void UpdateLivesCounter(int currentLives)
    {
        _livesCounter.text = currentLives.ToString();
    }

    public void InstaniateHeartsUI(float noOfHearts)
    {
        if (_heartList.Count == 0)
        {
            for (int i = 0; i < noOfHearts; i++)
            {
                GameObject heart = Instantiate(_heartPrefab, _healthContainer.transform);
                _heartList.Add(heart);
            }
        }
    }

    public void HealHeartUI(float _currentHeart)
    {
        if (_heartList.Count == 0)
        {
            return;
        }

        _heartList[((int)_currentHeart) - 1].GetComponent<Animator>().SetTrigger("HeartGained");
    }

    public void DestroyHeartUI(float _currentHeart)
    {
        if (_heartList.Count == 0)
        {
            Debug.LogWarning("Heart List is empty");
            return;
        }

        _heartList[((int)_currentHeart)].GetComponent<Animator>().SetTrigger("HeartLost");
    }

    public void ResetHeartUI()
    {
        if (_heartList.Count == 0)
        {
            return;
        }

        foreach (GameObject heart in _heartList)
        {
            heart.GetComponent<Animator>().SetTrigger("HeartGained");
        }
    }

    // Indicator UI Logic

    public void EnableIndicator(IndicatorType type)
    {
        if (_indicatorSpriteList.Length == 0)
        {
            Debug.LogWarning("Indicator Sprite List is Empty");
            return;
        }

        switch (type)
        {
            case IndicatorType.Shoot:
                _shootIndicator.sprite = _indicatorSpriteList[0];
                break;
            case IndicatorType.Punch:
                _punchIndicator.sprite = _indicatorSpriteList[1];
                break;
            case IndicatorType.Special:
                _specialIndicator.sprite = _indicatorSpriteList[2];
                break;
        }
    }

    public void DisableIndicator(IndicatorType type)
    {
        if (_indicatorSpriteList.Length == 0 || _indicatorSpriteList.Length != 6)
        {
            Debug.LogWarning("Indicator Sprite List is Empty");
            return;
        }

        switch (type)
        {
            case IndicatorType.Shoot:
                _shootIndicator.sprite = _indicatorSpriteList[3];
                break;
            case IndicatorType.Punch:
                _punchIndicator.sprite = _indicatorSpriteList[4];
                break;
            case IndicatorType.Special:
                _specialIndicator.sprite = _indicatorSpriteList[5];
                break;
        }
    }

    // Stamina UI Logic
    public void UpdateStamina(float currentStamina, float maxStamina)
    {
        _staminaSlider.value = currentStamina / maxStamina;
    }

    public void UpdateStaminaColor(bool isOutOfStamina)
    {
        Image fillRect = _staminaSlider.fillRect.gameObject.GetComponent<Image>();
        if (_normalStaminaColor == null && _outOfStaminaColor == null)
        {
            return;
        }

        fillRect.color = (isOutOfStamina) ? _outOfStaminaColor : _normalStaminaColor;
    }
}
