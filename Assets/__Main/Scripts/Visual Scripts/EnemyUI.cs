using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    private Camera _mainCamera;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private GameObject _healtSliderContainer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        transform.rotation = _mainCamera.transform.rotation;
    }

    public void UpdateHealthSlider(float _currentHealth, float _maxHealth)
    {
        _healthSlider.value = _currentHealth / _maxHealth;
    }

    public void ShowHealthSlider()
    {
        _healtSliderContainer.SetActive(true);
    }

    public void HideHealthSlider()
    {
        _healtSliderContainer.SetActive(false);
    }

}
