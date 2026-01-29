using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void StartGameM()
    {
        //AudioManager.instance.PlayGameMusic();

        SceneManager.LoadScene("SampleScene");
    }
    public void StartGameO()
    {
       //AudioManager.instance.PlayGameMusic();

        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
