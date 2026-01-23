using Unity.Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 3;
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxLives = 99f;
    [SerializeField] private float currentLives;
    [SerializeField] private bool _isInvincible = false;


    private PlayerMovement _pMovement;

    [SerializeField] private AudioSource _damageAudioSource;
    [SerializeField] private AudioClip[] _damageAudioClips;


    private void Awake()
    {
        _pMovement = GetComponent<PlayerMovement>();
    }

    private void Start() // Set Player Health and update UI
    {
        currentHealth = maxHealth;
        //UIManger.instance.UpdateHealth(currentHealth, maxHealth);
    }


    public void takeDamage(float damage) //Take Damage from Enemy.
    {
        currentHealth -= damage;
        _damageAudioSource.PlayOneShot(_damageAudioClips[1]);

        if (currentHealth <= 0)
        {
            onDeath();
            currentHealth = 0;
        }

        //UIManger.instance.UpdateHealth(currentHealth, maxHealth);
    }

    public void AddHealthPoint() //Heal when taking a Heart.
    {
        currentHealth++;
        _damageAudioSource.PlayOneShot(_damageAudioClips[2]);
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        //UIManger.instance.UpdateHealth(currentHealth, maxHealth);
    }

    public void AddLifePoint()
    {
        currentLives++;
        _damageAudioSource.PlayOneShot(_damageAudioClips[3]);
    }

    private void RemoveLifePoint()
    {
        currentLives--;
        _damageAudioSource.PlayOneShot(_damageAudioClips[4]);
    }

    public void onDeath() //On PLayer's death disable movement of all kind, and show lose screen.
    {
        if (_pMovement != null)
        {
            if(currentLives <= 0)
            {
                //_pMovement.canMove = false;
                _damageAudioSource.PlayOneShot(_damageAudioClips[0]);
                Time.timeScale = 0f;
                //UIManger.instance.LoseScreen();
            }
            else
            {
                RemoveLifePoint();
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
        return maxHealth;
    }

    public float getCurrentHealth()
    {
        return currentHealth;
    }

    public float getMaxLives()
    {
        return maxLives;
    }

    public float getCurrentLives()
    {
        return maxHealth;
    }


}
