using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;


public class Door : MonoBehaviour
{
    public Transform doorPivot;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen = false;
    private bool isMoving = false;

    private Quaternion closedRot;
    private Quaternion openRot;
    //for testing the code
    public bool test; 

    void Start()
    {
        closedRot = doorPivot.localRotation;
        openRot = Quaternion.Euler(0, openAngle, 0) * closedRot;
    }

    private void OnEnable()
    {
        PlayerInventory.OnMainCollectiblesCollected += OpenDoor;
    }

    private void OnDisable()
    {
        PlayerInventory.OnMainCollectiblesCollected -= OpenDoor;
    }
    public void OpenDoor()
    {
        if (isOpen || isMoving) return;
        StartCoroutine(RotateDoor(openRot));
        isOpen = true;
    }

    public void CloseDoor()
    {
        if (!isOpen || isMoving) return;
        StartCoroutine(RotateDoor(closedRot));
        isOpen = false;
    }

    IEnumerator RotateDoor(Quaternion targetRot)
    {
        isMoving = true;
        Quaternion startRot = doorPivot.localRotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorPivot.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        isMoving = false;
    }

    //for testing the code
    private void Update()
    {
        if (test) OpenDoor();
    }

    //can be used in other sicrpts 

    //public Door door; 

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //        door.OpenDoor();
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //        door.CloseDoor();
    //}



}
