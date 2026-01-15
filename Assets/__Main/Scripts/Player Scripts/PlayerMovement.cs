using UnityEngine;
using static GameEnums;

public class PlayerMovement : MonoBehaviour
{
    public CharacterType type;
    [Header("Basic Movement")]
    public PlayerMovementState movementState = PlayerMovementState.Movement;
    public float speed = 10;
    public float runSpeed = 15;
    public float crouchSpeed = 5;
    public float maxStamina = 20;
    public float currentStamina = 20;
    public float jumpForce = 5;
    public float doubleJumpForce = 3;
    public float rotationSpeed = 3;
    public float NormalHeight = 1f;
    public float crouchHeight = 0.6f;
    public bool canMove = true;

    [Header("Saved Values & Components")]
    private Rigidbody _theRigidBody;
    private Quaternion _targetRotation;
    private float _currentSpeed;
    private Vector3  _playerDirection;
    private Transform _cameraTransform;
    private CapsuleCollider _playerCollider;
    //private Animator _animator;

    [Header("Ground Checking")]
    [SerializeField] private float _groundCheckerOffset = -0.9f;
    [SerializeField] private float _groundCheckerRadius = 0.3f;
    [SerializeField] private LayerMask _groundLayer;
    [Header("Audio")]
    [SerializeField] private AudioSource[] _SFXSourceList;
    [SerializeField] private AudioClip[] _SFXClipList;
    [Header("Bool Checkers")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isCrouched = false;
    [SerializeField] private bool _isWalking;
    [SerializeField] private bool _isSprinting = false;
    [Header("Movement Controllers")]
    [SerializeField] private bool _canDoubleJump;
    [SerializeField] private bool _canSprint;
    [SerializeField] private bool _canUncrouch = true;
    [SerializeField] private bool _canDash = true;
    [Header("Bored Logic")]
    [SerializeField] private bool _isBored = false;
    [SerializeField] private float _boredTimer = 0;
    [SerializeField] private float _timeTillBored;
    [Header("Dashing Logic")]
    [SerializeField] private float _dashSpeed = 25f;
    [SerializeField] private float _dashExitSpeed = 0.6f;
    [SerializeField] private float _dashDuration = 0.15f;
    [SerializeField] private float _dashTimeLeft;
    [SerializeField] private float _dashTimer = 0;
    [SerializeField] private float _dashCooldown = 3f;
    [SerializeField] private TrailRenderer[] _dashTrailRendererList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created.

    private void Awake()
    {
        _theRigidBody = GetComponent<Rigidbody>(); //Getting Rigidbody from Player Object.
        _playerCollider = GetComponent<CapsuleCollider>();
        //_animator = GetComponent<Animator>();
        _cameraTransform = Camera.main.transform;
    }
    void Start()
    {

        _targetRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        _theRigidBody.freezeRotation = true; //This is to stop other game objects from affecting the player's rotation
        _currentSpeed = speed;
        currentStamina = maxStamina;

        //GameUIManager.instance.SetEnergyFill(currentStamina, maxStamina);
    }

    private void Update()
    {
        if (canMove) //if player can move (Not Dead) allow them to jump, sprint and crouch
        {
            jump();
            sprint();
            crouch();
            Dash();
            CheckDashTimer();
            CheckBoredTimer();
        }




    }

    // Update is called once per frame.
    void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }
        if (movementState == PlayerMovementState.Movement) //if player can move (Not Dead) allow them to move
        {
            moveAndRotate();
        }else if (movementState == PlayerMovementState.Dashing)
        {
            PerformDash();
        }



    }

    public void toggleUncroucher(bool toggle) // activate/deactivate ability to uncrouch
    {
        _canUncrouch = toggle;
    }

    private void moveAndRotate()
    {
        _isGrounded = Physics.CheckSphere(transform.position + Vector3.up * _groundCheckerOffset, _groundCheckerRadius, _groundLayer); //Checking if player is on ground.
        float Horizontal = Input.GetAxisRaw("Horizontal"); //Defining Char X Axis.
        float Vertical = Input.GetAxisRaw("Vertical"); //Defining Char Z Axis.

        _isWalking = ((Horizontal != 0 || Vertical != 0) && _isGrounded); //Check if player is walking to play walkingSFX
        //_animator.SetBool("isWalking", _isWalking);
        //_animator.SetBool("isGrounded", _isGrounded);

        if (_isWalking && !_SFXSourceList[0].isPlaying) //if player is walking and the walking audio source is not playing, play it.
        {
            _SFXSourceList[0].Play();
        }
        else if (!_isWalking && _SFXSourceList[0].isPlaying) //if player STOPPED walking and the walking audio source is playing, stop it.
        {
            _SFXSourceList[0].Stop();
            
        }

        //Stamina Checking
        if (_isWalking && _isSprinting)
        {
            currentStamina -= Time.deltaTime;
            //GameUIManager.instance.SetEnergyFill(currentStamina, maxStamina);
        }
        else if (!_isSprinting)
        {
            if(currentStamina < maxStamina)
            {
                currentStamina += Time.deltaTime;
                //GameUIManager.instance.SetEnergyFill(currentStamina, maxStamina);
            }
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

        //Rotation
        Vector3 lookDirection = movementDir; //Taking the Direction of the player

        if (lookDirection != Vector3.zero) //on player movement
        {
            _targetRotation = Quaternion.LookRotation(lookDirection); // makes the target rotation that we want the player to move to
        }

        _theRigidBody.MoveRotation(Quaternion.Lerp(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime)); //Using lerp to smooth the player rotation using current rotation, target rotaion and rotation speed.
    }

    private void jump()
    {
        // Allow Player to jump if on ground and jump button pressed.
        if (_isGrounded && !_isCrouched && Input.GetButtonDown("Jump"))
        {
            _theRigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _canDoubleJump = true;
            //_animator.SetTrigger("JumpTrigger");
            _SFXSourceList[1].PlayOneShot(_SFXClipList[3]);
        }
        // Allow Player to double jump if NOT on ground and jump button pressed.
        if (!_isGrounded && !_isCrouched && _canDoubleJump && Input.GetButtonDown("Jump"))
        {
            _theRigidBody.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
            _canDoubleJump = false;
            //_animator.SetTrigger("JumpTrigger");
            _SFXSourceList[1].PlayOneShot(_SFXClipList[4]);
        }


    }

    private void sprint()
    {
        //Sprint Code
        if (_isGrounded && !_isCrouched && _canSprint && Input.GetKey(KeyCode.LeftShift) && _isWalking)
        {
            _SFXSourceList[0].clip = _SFXClipList[2];
            _currentSpeed = runSpeed;
            _isSprinting = true;
            //_animator.SetBool("isSprinting", _isSprinting);
        }
        else if ((Input.GetKeyUp(KeyCode.LeftShift) || !_canSprint || !_isWalking) && !_isCrouched)
        {
            _SFXSourceList[0].clip = _SFXClipList[0];
            _currentSpeed = speed;
            _isSprinting = false;
            //_animator.SetBool("isSprinting", _isSprinting);
        }

        //Stamina Code
        if (currentStamina <= 0)
        {
            currentStamina = 0;
            _canSprint = false;
            _SFXSourceList[3].PlayOneShot(_SFXClipList[7]);
        }

        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
            _canSprint = true;
        }
    }

    private void crouch()
    {
        //Crouch Code
        if (_isGrounded && !_isCrouched && !_isSprinting && Input.GetKeyDown(KeyCode.LeftControl))
        {
            //_playerCollider.center = new Vector3(0f, 0.003449329f, 0f);
            //_playerCollider.height = crouchHeight;
            transform.localScale = new Vector3(1f, crouchHeight, 1f);
            _isCrouched = true;
            //_animator.SetBool("isCrouched", _isCrouched);
            _currentSpeed = crouchSpeed;
            _SFXSourceList[0].clip = _SFXClipList[1];

            if (_isWalking)
            {
                _SFXSourceList[0].Stop();
                _SFXSourceList[0].Play();
            }

            _SFXSourceList[2].PlayOneShot(_SFXClipList[5]);
        }
        else if (_isCrouched && _canUncrouch && Input.GetKeyDown(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(1f, NormalHeight, 1f);
            //_playerCollider.center = new Vector3(0f, 0.005028358f, 0f);
            //_playerCollider.height = NormalHeight;
            _isCrouched = false;
            //_animator.SetBool("isCrouched", _isCrouched);
            _currentSpeed = speed;
            _SFXSourceList[0].clip = _SFXClipList[0];
            if (_isWalking)
            {
                _SFXSourceList[0].Stop();
                _SFXSourceList[0].Play();
            }

            _SFXSourceList[2].PlayOneShot(_SFXClipList[6]);
        }
    }

    // Bored Logic
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
            //_animator.SetBool("isBored", _isBored);
        }

        if(!_isBored && _boredTimer >= _timeTillBored)
        {
            _isBored = true;
            //_animator.SetBool("isBored", _isBored);
        }
    }

    private void StartBoredTimer()
    {
        _boredTimer += Time.deltaTime;
    }

    //Dash Logic
    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Q) && _canDash && !_isCrouched && type == CharacterType.Mohammed)
        {
            movementState = PlayerMovementState.Dashing;
            _canDash = false;
            _dashTimeLeft = _dashDuration;
            ToggleDashTrails(true);
            _SFXSourceList[4].PlayOneShot(_SFXSourceList[4].clip);
        }
    }

    private void PerformDash()
    {
        Vector3 dashDirection = (_playerDirection != Vector3.zero) ? _playerDirection : transform.forward;
        _theRigidBody.linearVelocity = new Vector3(
            dashDirection.x * _dashSpeed,
            _theRigidBody.linearVelocity.y,
            dashDirection.z * _dashSpeed);

        _dashTimeLeft -= Time.fixedDeltaTime;

        if (_dashTimeLeft <= 0f)
        {
            movementState = PlayerMovementState.Movement;
            ToggleDashTrails(false);

            _theRigidBody.linearVelocity *= _dashExitSpeed;

        }

        
    }

    private void CheckDashTimer() {
        if (!_canDash) {
            StartDashRateTimer();
        }
        if (_dashTimer >= _dashCooldown && _isGrounded){
            _dashTimer = 0; _canDash = true;
        }
    }
    private void StartDashRateTimer()
    {

        _dashTimer += Time.deltaTime;

    }

    private void ToggleDashTrails(bool toggle)
    {
        foreach(TrailRenderer renderer in _dashTrailRendererList)
        {
            renderer.emitting = toggle;
        }
    }

    private void OnDrawGizmos() //Gizmo to draw the ground checker sphere.
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * _groundCheckerOffset, _groundCheckerRadius);
    }
}
