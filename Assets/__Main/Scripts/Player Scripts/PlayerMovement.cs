using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using static GameEnums;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    public CharacterType type;
    [Header("Movement")]
    [SerializeField] private PlayerMovementState movementState = PlayerMovementState.Movement;
    public float speed = 10;
    public float runSpeed = 15;
    public float crouchSpeed = 5;
    public float rotationSpeed = 3;

    [Header("Jump")]
    public float jumpForce = 5;
    public float doubleJumpForce = 3;

    [Header("Stamina")]
    public float maxStamina = 20;
    public float currentStamina = 20;

    [Header("Player Collider Size")]
    public float NormalHeight = 1f;
    public float crouchHeight = 0.6f;
    public float NormalCenter = 0f;
    public float crouchCenter = 0f;

    [Header("Ground Checking")]
    [SerializeField] private float _groundCheckerOffset = -0.9f;
    [SerializeField] private float _groundCheckerRadius = 0.3f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Special Ability Logic")]
    [SerializeField] private float _saSpeed = 25f;
    [SerializeField] private float _saExitSpeed = 0.6f;
    [SerializeField] private float _saDuration = 0.15f;
    [SerializeField] private float _saCooldown = 3f;
    [SerializeField] private TrailRenderer[] _saRendererList;
    [SerializeField] private GameObject GroundImpactPrefab;
    [SerializeField] private float _groundImpactOffset = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioSource[] _SFXSourceList;
    [SerializeField] private AudioClip[] _SFXClipList;

    [Header("Player Cameras")]
    [SerializeField] private CinemachineCamera _normalCamera;
    [SerializeField] private CinemachineCamera _crouchCamera;

    [Header("Testing")]
    [SerializeField] bool _isCapsule = false;

    //Components
    private Rigidbody _theRigidBody;
    private Transform _cameraTransform;
    private CapsuleCollider _playerCollider;
    private PlayerInputHandler _input;
    private Animator _animator;
    private Renderer[] _skinnedMeshRendererList;

    //Input States
    private Vector2 _moveInput;
    private bool _sprintHeld;

    // Runtime Changing Values

    [Header("Bool Checkers")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isCrouched = false;
    [SerializeField] private bool _isWalking;
    [SerializeField] private bool _isSprinting = false;
    [SerializeField] private bool _isUsingSpecial = false;

    [Header("Movement Controllers")]
    [SerializeField] private bool _canDoubleJump;
    [SerializeField] private bool _canSprint = true;
    [SerializeField] private bool _canUncrouch = true;
    [SerializeField] private bool _canUseSpecialAbility = true;

    [Header("Bored Logic")]
    [SerializeField] private bool _isBored = false;
    [SerializeField] private float _boredTimer = 0;
    [SerializeField] private float _timeTillBored = 10f;

    [Header("Cooldown and speed Logic")]
    [SerializeField] private float _saTimeLeft;
    [SerializeField] private float _saCooldownTimer;
    [SerializeField] private float _currentSpeed;

    private Quaternion _targetRotation;
    private Vector3 _playerDirection;


    // Start is called once before the first execution of Update after the MonoBehaviour is created.

    private void Awake()
    {
        _theRigidBody = GetComponent<Rigidbody>(); //Getting Rigidbody from Player Object.
        _playerCollider = GetComponent<CapsuleCollider>();
        _animator = GetComponent<Animator>();
        _skinnedMeshRendererList = GetComponentsInChildren<SkinnedMeshRenderer>();
        _cameraTransform = Camera.main.transform;
        _input = GetComponent<PlayerInputHandler>();

    }
    void Start()
    {

        _targetRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _theRigidBody.freezeRotation = true; //This is to stop other game objects from affecting the player's rotation
        _currentSpeed = speed;
        currentStamina = maxStamina;

        GameUIManager.instance.UpdateStamina(currentStamina, maxStamina);
    }


    // Input Handlers (New Input System)

    private void OnEnable()
    {
        _input.OnMove += v => _moveInput = v;
        _input.OnJump += HandleJump;
        _input.OnSprint += held => _sprintHeld = held;
        _input.OnCrouch += HandleCrouch;
        _input.OnSpecial += HandleSA;
        _input.OnPause += PauseUnpauseGame;
    }

    private void OnDisable()
    {
        _input.OnMove -= v => _moveInput = v;
        _input.OnJump -= HandleJump;
        _input.OnSprint -= held => _sprintHeld = held;
        _input.OnCrouch -= HandleCrouch;
        _input.OnSpecial -= HandleSA;
        _input.OnPause -= PauseUnpauseGame;
    }

    private void Update()
    {

        if(movementState == PlayerMovementState.Disabled)
        {
            return;
        }

        CheckGround();
        HandleSprint();
        HandleCooldowns();
        CheckBoredTimer();


    }

    // Update is called once per frame.
    void FixedUpdate()
    {

        if (movementState == PlayerMovementState.Disabled)
        {
            return;
        }

        switch (movementState)
        {
            case PlayerMovementState.Movement:
                moveAndRotate();
                break;
            case PlayerMovementState.Dashing:
                PerformDash();
                break;
            case PlayerMovementState.Bashing:
                PerformBash();
                break;
            default:
                break;
        }



    }

    // Getters
    public bool GetIsCrouched()
    {
        return _isCrouched;
    }

    public bool GetIsGrounded()
    {
        return _isGrounded;
    }

    public PlayerMovementState GetMovementState()
    {
        return movementState;
    }

    //Setters
    public void SetTargetRotation(Quaternion targetRotaion)
    {
        _targetRotation = targetRotaion;
    }

    // Movement Logic
    private void moveAndRotate()
    {

        float Horizontal = _moveInput.x; //Defining Char X Axis using the new input system.
        float Vertical = _moveInput.y; //Defining Char Z Axis using the new input system.

        _isWalking = _moveInput.sqrMagnitude > 0.01f && _isGrounded; //Check if player is walking to play walkingSFX
        _animator.SetBool("isWalking", _isWalking);
        _animator.SetBool("isGrounded", _isGrounded);

        if (_isWalking && !_SFXSourceList[0].isPlaying) //if player is walking and the walking audio source is not playing, play it.
        {
            _SFXSourceList[0].Play();
        }
        else if (!_isWalking && _SFXSourceList[0].isPlaying) //if player STOPPED walking and the walking audio source is playing, stop it.
        {
            _SFXSourceList[0].Stop();

        }

        // Camera Controls (for Realtive Movement)
        // Taking the Camera Forward and Right
        Vector3 cameraForward = _cameraTransform.forward;
        Vector3 cameraRight = _cameraTransform.right;

        //freezing the camera's y axis as we don't want it to be affected for the direction
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        //Realtive Cam Direction
        Vector3 forwardRealtive = Vertical * cameraForward;
        Vector3 rightRealtive = Horizontal * cameraRight;

        _playerDirection = (forwardRealtive + rightRealtive).normalized;

        Vector3 movementDir = _playerDirection * _currentSpeed; //assigning movement with camera direction in mind, also using normalized to make movement in corner dierctions the same as normal directions (not faster)

        //Movement
        _theRigidBody.linearVelocity = new Vector3(movementDir.x, _theRigidBody.linearVelocity.y, movementDir.z); // Changing the velocity based on Horizontal and Vertical Movements alongside camera direction.


        if (_playerDirection != Vector3.zero) //on player movement
        {
            _targetRotation = Quaternion.LookRotation(_playerDirection); // makes the target rotation that we want the player to move to
            
        }
        _theRigidBody.MoveRotation(Quaternion.Lerp(transform.rotation, _targetRotation, rotationSpeed * Time.fixedDeltaTime)); //Using lerp to smooth the player rotation using current rotation, target rotaion and rotation speed.

    }



    public void DisableMovement()
    {
        movementState = PlayerMovementState.Disabled;
        _isSprinting = false;
        _isUsingSpecial = false;
        _canUseSpecialAbility = false;

        _theRigidBody.linearVelocity = Vector3.zero;
        UpdateTrails();
    }

    public void EnableMovement()
    {
        movementState = PlayerMovementState.Movement;
        _canUseSpecialAbility = true;
    }

    // Jump Logic =======================================================================

    private void HandleJump()
    {
        if (_isCrouched || movementState == PlayerMovementState.Disabled) return;
        // Allow Player to jump if on ground and jump button pressed.
        if (_isGrounded)
        {
            _theRigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _canDoubleJump = true;
            _animator.SetTrigger("JumpTrigger");
            _SFXSourceList[1].PlayOneShot(_SFXClipList[3]);
        }
        // Allow Player to double jump if NOT on ground and jump button pressed.
        else if (_canDoubleJump)
        {
            _theRigidBody.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
            _canDoubleJump = false;
            _animator.SetTrigger("JumpTrigger");
            _SFXSourceList[1].PlayOneShot(_SFXClipList[4]);
        }


    }

    // Sprint Logic =======================================================================

    private void HandleSprint()
    {
        _isSprinting = _isGrounded && !_isCrouched && _canSprint && _sprintHeld && _isWalking;
        _currentSpeed = _isSprinting ? runSpeed : (_isCrouched ? crouchSpeed : speed);

        //Sprint Code
        if (_isSprinting)
        {
            currentStamina -= Time.deltaTime;

            //Stamina Code
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                _canSprint = false;
                _SFXSourceList[3].PlayOneShot(_SFXClipList[7]);
                GameUIManager.instance.UpdateStaminaColor(true);
            }

            _SFXSourceList[0].clip = _SFXClipList[2];
            GameUIManager.instance.UpdateStamina(currentStamina, maxStamina);
            _animator.SetBool("isSprinting", _isSprinting);
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += Time.deltaTime;
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                _canSprint = true;
                GameUIManager.instance.UpdateStaminaColor(false);
            }
            _SFXSourceList[0].clip = _SFXClipList[0];
            GameUIManager.instance.UpdateStamina(currentStamina, maxStamina);
            _animator.SetBool("isSprinting", _isSprinting);
        }

        UpdateTrails();
    }

    // Crouch Logic =======================================================================

    private void HandleCrouch()
    {
        if( movementState == PlayerMovementState.Disabled)
        {
            return;
        }

        //Crouch Code
        if (_isGrounded && !_isCrouched && !_isSprinting)
        {
            
            if (_isCapsule)
            {
                transform.localScale = new Vector3(1f, crouchHeight, 1f);
            }
            else
            {
                _playerCollider.center = new Vector3(0f, crouchCenter, 0f);
                _playerCollider.height = crouchHeight;
            }
            _isCrouched = true;
            _crouchCamera.Priority = 1;
            _normalCamera.Priority = 0;
            DeactivateMesh();
            _animator.SetBool("isCrouched", _isCrouched);
            _SFXSourceList[0].clip = _SFXClipList[1];
            _SFXSourceList[2].PlayOneShot(_SFXClipList[5]);
        }
        else if (_isCrouched && _canUncrouch)
        {

            if (_isCapsule)
            {
                transform.localScale = new Vector3(1f, NormalHeight, 1f);
            }
            else
            {
                _playerCollider.center = new Vector3(0f, NormalCenter, 0f);
                _playerCollider.height = NormalHeight;
            }
            _isCrouched = false;
            _normalCamera.Priority = 1;
            _crouchCamera.Priority = 0;
            ActivateMesh();
            _animator.SetBool("isCrouched", _isCrouched);
            _currentSpeed = speed;
            _SFXSourceList[0].clip = _SFXClipList[0];

            _SFXSourceList[2].PlayOneShot(_SFXClipList[6]);
        }
    }

    public void toggleUncroucher(bool toggle) // activate/deactivate ability to uncrouch
    {
        _canUncrouch = toggle;
    }

    // Bored Logic =======================================================================
    private void CheckBoredTimer()
    {
        if (!_isWalking)
        {
            StartBoredTimer();
        }
        else if (_boredTimer > 0)
        {
            _boredTimer = 0;
            _isBored = false;
            _animator.SetBool("isBored", _isBored);
        }

        if(!_isBored && _boredTimer >= _timeTillBored)
        {
            _isBored = true;
            int RandomIdle = Random.Range(0, 3);
            _animator.SetFloat("Bored_Idle", RandomIdle);
            _animator.SetBool("isBored", _isBored);
        }
    }

    private void StartBoredTimer()
    {
        _boredTimer += Time.deltaTime;
    }

    // Special Abilities ==============================================================================================================================================

    private void HandleSA()
    {
        if (!_canUseSpecialAbility || _isCrouched)
        {
            return;
        }

        if (type == CharacterType.Mohammed)
        {
            StartDash();
        }
        else if (type == CharacterType.Omar && !_isGrounded)
        {
            StartBash();
        }
    }

    private void HandleCooldowns()
    {
        if(_canUseSpecialAbility)
        {
            return;
        }

        _saCooldownTimer += Time.deltaTime;
        if(_saCooldownTimer >= _saCooldown && _isGrounded)
        {
            _saCooldownTimer = 0;
            _canUseSpecialAbility = true;
            GameUIManager.instance.EnableIndicator(IndicatorType.Special);
        }
    }

    private void UpdateTrails()
    {
        if(_saRendererList.Length == 0)
        {
            return;
        }
        bool active = _isSprinting || _isUsingSpecial;
        foreach (TrailRenderer renderer in _saRendererList)
        {
            renderer.emitting = active;
        }
    }

    // Water Dash Logic =======================================================================
    private void StartDash()
    {
        movementState = PlayerMovementState.Dashing;
        _canUseSpecialAbility = false;
        _isUsingSpecial = true;
        _saTimeLeft = _saDuration;
        _SFXSourceList[4].PlayOneShot(_SFXSourceList[4].clip);
        UpdateTrails();
        _animator.SetBool("isUsingSA", true);
        GameUIManager.instance.DisableIndicator(IndicatorType.Special);
    }

    private void PerformDash()
    {
        Vector3 dashDirection = (_playerDirection != Vector3.zero) ? _playerDirection : transform.forward;
        _theRigidBody.linearVelocity = new Vector3(
            dashDirection.x * _saSpeed,
            _theRigidBody.linearVelocity.y,
            dashDirection.z * _saSpeed);
        
        FinishSA();

        
    }
    // Fiery Ground Bash =======================================================================

    private void StartBash()
    {
        movementState = PlayerMovementState.Bashing;
        _canUseSpecialAbility = false;
        _isUsingSpecial = true;
        _saTimeLeft = _saDuration;
        _SFXSourceList[4].PlayOneShot(_SFXSourceList[4].clip);
        UpdateTrails();
        _animator.SetBool("isUsingSA", true);
        GameUIManager.instance.DisableIndicator(IndicatorType.Special);
    }

    private void PerformBash()
    {
        _theRigidBody.linearVelocity = new Vector3(_theRigidBody.linearVelocity.x
            , Vector3.down.y * _saSpeed,
            _theRigidBody.linearVelocity.z
            );

        FinishSA();
    }

    private void FinishSA()
    {
        if(type == CharacterType.Mohammed)
        {
            _saTimeLeft -= Time.fixedDeltaTime;

            if (_saTimeLeft <= 0)
            {
                _isUsingSpecial = false;
                movementState = PlayerMovementState.Movement;
                _theRigidBody.linearVelocity *= _saExitSpeed;
                UpdateTrails();
                _animator.SetBool("isUsingSA", false);
            }
        }
        else if (type == CharacterType.Omar)
        {
            if (_isGrounded)
            {
                _isUsingSpecial = false;
                movementState = PlayerMovementState.Movement;
                _theRigidBody.linearVelocity *= _saExitSpeed;
                UpdateTrails();
                Instantiate(GroundImpactPrefab, (transform.position + Vector3.up * _groundImpactOffset), Quaternion.identity);
                _animator.SetBool("isUsingSA", false);
            }
        }
    }

    //Helper Code

    private void PauseUnpauseGame()
    {
        GameUIManager.instance.TogglePauseMenu();
    }
    private void CheckGround()
    {
        _isGrounded = Physics.CheckSphere(transform.position + Vector3.up * _groundCheckerOffset, _groundCheckerRadius, _groundLayer); //Checking if player is on ground.
    }

    private void DeactivateMesh()
    {
        foreach (Renderer mesh in _skinnedMeshRendererList)
        {
            mesh.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }

    private void ActivateMesh()
    {
        foreach (Renderer mesh in _skinnedMeshRendererList)
        {
            mesh.shadowCastingMode = ShadowCastingMode.On;
        }
    }

    public void UpdateRendererList()
    {
        _skinnedMeshRendererList = GetComponentsInChildren<Renderer>();
    }
    // Gizmos =======================================================================
    private void OnDrawGizmos() //Gizmo to draw the ground checker sphere.
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * _groundCheckerOffset, _groundCheckerRadius);
    }
}
