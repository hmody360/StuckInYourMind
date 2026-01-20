using UnityEngine;
using UnityEngine.Events;

public class GroundButton : MonoBehaviour
{
    [Header("Button Settings")]
    public UnityEvent OnPressed;

    private bool pressed = false;

    void OnTriggerEnter(Collider other)
    {
        if (pressed) return;

        if (other.CompareTag("Player"))
        {
            pressed = true;
            OnPressed.Invoke();
        }
    }
}
