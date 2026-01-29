using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 3;
    [SerializeField] private float _currentHealth;
    [SerializeField] private int _maxLives = 99;
    [SerializeField] private int _currentLives;
    [SerializeField] private float _invincibilityTime = 0;
    [SerializeField] private bool _isInvincible = false;
    [SerializeField] private Vector3 _currentCheckPoint;
    [SerializeField] private GameObject _respawnEffect;


    private PlayerMovement _pMovement;
    private PlayerAttack _pAttack;
    private Animator _pAnimator;
    private Renderer[] _skinnedMeshRendererList;

    [SerializeField] private AudioSource _damageAudioSource;
    [SerializeField] private AudioClip[] _damageAudioClips;

    public static event Action<Vector3> OnCheckPointSet;


    private void Awake()
    {
        _pMovement = GetComponent<PlayerMovement>();
        _pAttack = GetComponent<PlayerAttack>();
        _pAnimator = GetComponent<Animator>();
        _skinnedMeshRendererList = GetComponentsInChildren<Renderer>();
        _currentCheckPoint = transform.localPosition;
    }

    private void Start() // Set Player Health and update UI
    {
        _currentHealth = _maxHealth;
        _currentLives = 3;
        GameUIManager.instance.UpdateLivesCounter(_currentLives);
        GameUIManager.instance.InstaniateHeartsUI(_maxHealth);
    }


    public void takeDamage(float damage) //Take Damage from Enemy.
    {
        if (_isInvincible)
        {
            return;
        }

        _currentHealth -= damage;
        _damageAudioSource.PlayOneShot(_damageAudioClips[0]);

        if (_currentHealth <= 0)
        {
            onDeath();
        }
        else
        {
            _pAnimator.SetTrigger("DamageTrigger");
            GameUIManager.instance.DestroyHeartUI(_currentHealth);
            StartCoroutine(DamageTimer());
            StartCoroutine(InvincibleEffect());
            
        }
    }

    public bool AddHealthPoint() //Heal when taking a Heart.
    {
        if (_currentHealth >= _maxHealth)
        {
            return false;
        }
        else
        {
            _currentHealth++;
            _damageAudioSource.PlayOneShot(_damageAudioClips[1]);
            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
            GameUIManager.instance.HealHeartUI(_currentHealth);
            return true;
        }

    }

    public bool AddLifePoint()
    {
        if (_currentLives >= _maxLives)
        {
            return false;
        }
        else
        {
            _currentLives++;
            _damageAudioSource.PlayOneShot(_damageAudioClips[2]);
            GameUIManager.instance.UpdateLivesCounter(_currentLives);
            return true;

        }

    }

    private void RemoveLifePoint()
    {
        _currentLives--;
        _damageAudioSource.PlayOneShot(_damageAudioClips[3]);
        GameUIManager.instance.UpdateLivesCounter(_currentLives);
    }

    public void onDeath() //On PLayer's death disable movement of all kind, and show lose screen.
    {
        if (_pMovement != null)
        {
            if (_currentLives <= 0)
            {
                StartCoroutine(DeathSequence());
                _currentHealth = 0;
            }
            else
            {
                RemoveLifePoint();
                Respawn();
                // Update Lives in UI & Show Lives UI for a bit then disapper
            }

        }
    }

    public void destroyAllEnemies() //Destroy All Enemies, this is used on player's death.
    {
        GameObject[] enemiesList = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemiesList)
        {
            Destroy(enemy);
        }
    }

    public float getMaxHealth()
    {
        return _maxHealth;
    }

    public float getCurrentHealth()
    {
        return _currentHealth;
    }

    public float getMaxLives()
    {
        return _maxLives;
    }

    public float getCurrentLives()
    {
        return _maxHealth;
    }

    private void Respawn()
    {
        _currentHealth = _maxHealth;
        _isInvincible = false;
        if (_currentCheckPoint != null && _respawnEffect != null)
        {
            transform.localPosition = _currentCheckPoint;
            _pAnimator.SetTrigger("RespawnTrigger");
            _respawnEffect.SetActive(true);
        }
        else
        {
            Debug.Log("Respawn point or respawn effect are null");
        }

        GameUIManager.instance.ResetHeartUI();
    }

    public void SetCheckpoint(Vector3 position)
    {
        _currentCheckPoint = position;
        OnCheckPointSet?.Invoke(_currentCheckPoint);
    }

    public Vector3 GetCheckPoint()
    {
        return _currentCheckPoint;
    }

    public void UpdateRendererList()
    {
        _skinnedMeshRendererList = GetComponentsInChildren<Renderer>();
    }

    private IEnumerator DamageTimer()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_invincibilityTime);
        _isInvincible = false;
    }

    private IEnumerator InvincibleEffect()
    {
        while (_isInvincible)
        {
            foreach (Renderer smr in _skinnedMeshRendererList)
            {
                smr.enabled = false;
            }

            yield return new WaitForSeconds(.1f);
            foreach (Renderer smr in _skinnedMeshRendererList)
            {
                smr.enabled = true;
            }
            yield return new WaitForSeconds(.1f);
        }
    }



    private IEnumerator DeathSequence()
    {
        _pMovement.DisableMovement();
        _pAttack.DisableOffense();
        _damageAudioSource.PlayOneShot(_damageAudioClips[4]);
        _pAnimator.SetBool("isDead", true);
        yield return new WaitForSeconds(3f);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        GameUIManager.instance.ShowGameOverMenu();
    }
}
