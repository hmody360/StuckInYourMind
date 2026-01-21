using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform doorPivot;   
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public float interactDistance = 2f;

    public bool canOpenDoor = false; //will be changed later

    private bool isOpen = false;
    private bool isMoving = false;

    void Update()
    {
        if (isMoving) return;
        if (!canOpenDoor) return;   

        if (Input.GetKeyDown(KeyCode.E) && PlayerInRange())
        {
            if (!isOpen)
                StartCoroutine(RotateDoor(Quaternion.Euler(0, openAngle, 0)));
            else
                StartCoroutine(RotateDoor(Quaternion.Euler(0, 0, 0)));

            isOpen = !isOpen;
        }
    }

    bool PlayerInRange()
    {
        return Vector3.Distance(
            Camera.main.transform.position,
            doorPivot.position
        ) <= interactDistance;
    }

    System.Collections.IEnumerator RotateDoor(Quaternion targetRot)
    {
        isMoving = true;
        Quaternion startRot = doorPivot.localRotation;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * openSpeed;
            doorPivot.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        isMoving = false;
    } 

    //these is aunthor logic for the door
}
