using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isGamePaused = false;

    public static GameManager instance;

    private PlayerMovement _playerMovement;
    private PlayerAttack _playerAttack;
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        _playerMovement.DisableMovement();
        _playerAttack.DisableOffense();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _playerMovement.EnableMovement();
        _playerAttack.EnableOffense();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "PlayerTestScene") //What To Do on level scene
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            _playerMovement = player.GetComponent<PlayerMovement>();
            _playerAttack = player.GetComponent<PlayerAttack>();
        }
    }
}
