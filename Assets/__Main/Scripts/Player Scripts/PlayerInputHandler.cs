using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public event Action<Vector2> OnMove;
    public event Action OnJump;
    public event Action<bool> OnSprint;
    public event Action onCoruch;
    public event Action onSpecial;

    private PlayerInputActions _input;


    private void Awake()
    {
        _input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _input.Player.Enable();

        _input.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        _input.Player.Move.canceled += _ => OnMove?.Invoke(Vector2.zero);

        _input.Player.Jump.performed += _ => OnJump?.Invoke();

        _input.Player.Sprint.performed += _ => OnSprint?.Invoke(true);
        _input.Player.Sprint.canceled += _ => OnSprint?.Invoke(false);

        _input.Player.Crouch.performed += _ => onCoruch?.Invoke();

        _input.Player.Special.performed += _ => onSpecial?.Invoke();
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }
}

