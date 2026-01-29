using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void StartGame()
    {
        //AudioManager.instance.PlayGameMusic();

        SceneManager.LoadScene("SampleScene");
    }

  
    public void ExitGame()
    {
        Application.Quit();
    }
}
