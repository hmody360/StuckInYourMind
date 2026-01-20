using Unity.Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth;
    public float currentHealth;

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


    public void takeDamage(float damage) //Take Damage from Enemy and Bad Health Vials.
    {
        currentHealth -= damage;
        _damageAudioSource.PlayOneShot(_damageAudioClips[0]);

        if (currentHealth <= 0)
        {
            onDeath();
            currentHealth = 0;
        }

        //UIManger.instance.UpdateHealth(currentHealth, maxHealth);
    }

    public void heal(float healAmount) //Heal when taking a good Health Vial.
    {
        currentHealth += healAmount;
        _damageAudioSource.PlayOneShot(_damageAudioClips[2]);
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        //UIManger.instance.UpdateHealth(currentHealth, maxHealth);
    }

    public void onDeath() //On PLayer's death disable movement of all kind, and show lose screen.
    {
        if (_pMovement != null)
        {
            //_pMovement.canMove = false;
            _damageAudioSource.PlayOneShot(_damageAudioClips[1]);
            Time.timeScale = 0f;
            //UIManger.instance.LoseScreen();
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


}
