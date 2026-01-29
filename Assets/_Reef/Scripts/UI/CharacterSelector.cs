
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickTrigger : MonoBehaviour
{

    public CameraControl manager;


    public bool isMohammed;
    public bool isOmar;

    void OnMouseEnter()
    {

    }

    void OnMouseDown()
    {
        if (isMohammed)
        {
            manager.GoToMohammedCam();
            StartCoroutine(GoToPlayerScene("MohammedPlayScene"));
        }
        else if (isOmar)
        {
            manager.GoToOmarCam();
            StartCoroutine(GoToPlayerScene("OmarPlayScene"));
        }
        else
        {

            manager.GoToMain();
        }
    }

    private IEnumerator GoToPlayerScene(string sceneName)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
    }
}