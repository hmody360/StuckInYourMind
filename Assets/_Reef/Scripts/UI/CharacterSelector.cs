
using UnityEngine;

public class ClickTrigger : MonoBehaviour
{
    
    public CameraControl manager;

    
    public bool isCharacter2;
    public bool isCharacter3;

    void OnMouseEnter()
    {
     
    }

    void OnMouseDown()
    {
        if (isCharacter2)
        {
            manager.GoToCam4();
        }
        else if (isCharacter3)
        {
            manager.GoToCam3();
        }
        else
        {
           
            manager.GoToMain();
        }
    }
}