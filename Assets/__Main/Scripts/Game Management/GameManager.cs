using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isGamePaused = false;
    [SerializeField] private int _collectedSecretCollectibles = 0;

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
        GameUIManager.OnGamePaused += PauseGame;
        GameUIManager.OnGameResumed += ResumeGame;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameUIManager.OnGamePaused -= PauseGame;
        GameUIManager.OnGameResumed -= ResumeGame;
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

    public void RestartCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        Time.timeScale = 1f;
        _collectedSecretCollectibles = 0;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        Time.timeScale = 1f;
        _collectedSecretCollectibles = 0;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlayerTestScene") //What To Do on level scene
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            _playerMovement = player.GetComponent<PlayerMovement>();
            _playerAttack = player.GetComponent<PlayerAttack>();
        }
    }
    //============== Game Logic ====================
    public void AddSecretCollectible()
    {
        _collectedSecretCollectibles++;
    }

}
