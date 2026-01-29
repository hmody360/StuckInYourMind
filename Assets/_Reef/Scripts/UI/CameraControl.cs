using UnityEngine;
using Unity.Cinemachine; 

public class CameraControl : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineCamera mainCam;
    public CinemachineCamera cam2;
    public CinemachineCamera cam3;

    [Header("UI Canvases")]
    public GameObject mainCanvas;
    public GameObject canvas2;
    public GameObject canvas3;
     
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GoToMain();
    }

    public void GoToCam2()
    {
        SetCameraPriorities(cam2);
        UpdateUI(canvas2);
    }

    public void GoToCam3()
    {
        SetCameraPriorities(cam3);
        UpdateUI(canvas3);
    }

    public void GoToMain()
    {
        SetCameraPriorities(mainCam);
        UpdateUI(mainCanvas);
    }

    private void SetCameraPriorities(CinemachineCamera activeCam)
    {
        mainCam.Priority = (activeCam == mainCam) ? 10 : 0;
        cam2.Priority = (activeCam == cam2) ? 10 : 0;
        cam3.Priority = (activeCam == cam3) ? 10 : 0;
    }

    //Shows target canvas, hides others
    private void UpdateUI(GameObject activeCanvas)
    {
        mainCanvas.SetActive(activeCanvas == mainCanvas);
        canvas2.SetActive(activeCanvas == canvas2);
        canvas3.SetActive(activeCanvas == canvas3);
    }
}