using UnityEngine;
using static GameEnums;
public class PlayerAttack : MonoBehaviour
{
    [Header("General Modfiers")]
    public bool canAttack = true;
    public bool canShoot = true;
    private PlayerOffenseState _offenseState = PlayerOffenseState.Neutral;

    [Header("Attack Modfiers")]
    [SerializeField] private bool _attackPressed;
    [SerializeField] private GameObject _attackArmPrefab;
    [SerializeField] private float _attackTimeLeft;
    [SerializeField] private float _attackDuration = 0.15f;
    [SerializeField] private float _attackCooldown = 3f;
    [SerializeField] private float _attackCooldownTimer;
    [SerializeField] private float _attackDamage = 5f;


    [Header("Shoot Modfiers")]
    [SerializeField] private bool _shootPressed;
    [SerializeField] private float _shotPointForwardOffset = 2f;
    [SerializeField] private float _shotPointUpOffset = 0.3f;
    [SerializeField] private float _shotPointRightOffset = 1f;
    [SerializeField] private GameObject _shotPrefab;
    [SerializeField] private float _shootDamage = 6f;
    [SerializeField] private float _shootForce = 1f;
    [SerializeField] private float _shootCooldown = 0.50f;
    [SerializeField] private float _shootCooldownTimer;

    [Header("Audio")]
    [SerializeField] private AudioSource[] _audioSources;
    [SerializeField] private AudioClip[] _audioClips;

    [Header("bool Indicators")]
    [SerializeField] private bool _isAttacking;
    [SerializeField] private bool _isShooting;
    //Components
    //private Animator _animator;
    private PlayerInputHandler _input;
    private PlayerMovement _playerMovement;
    private void Awake()
    {
        //_animator = GetComponent<Animator>();
        _input = GetComponent<PlayerInputHandler>();
        _playerMovement = GetComponent<PlayerMovement>();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_attackArmPrefab != null && _attackArmPrefab.GetComponent<AttackImpact>() != null && _attackArmPrefab.GetComponent<Collider>() != null)
        {
            _attackArmPrefab.GetComponent<AttackImpact>().attackPower = _attackDamage;
            _attackArmPrefab.GetComponent<Collider>().enabled = false;
        }

        if (_shotPrefab.GetComponent<ShotImpact>() != null)
        {
            _shotPrefab.GetComponent<ShotImpact>().shotDamage = _shootDamage;
        }

    }

    private void Update()
    {
        if (_offenseState == PlayerOffenseState.Disabled)
        {
            ResetFrameInputs();
            return;
        }

        HandleAttack();
        HandleShooting();
        HandleAttackCooldowns();
        HandleShootCooldowns();
        ResetFrameInputs();
    }

    private void FixedUpdate()
    {
        if (_offenseState == PlayerOffenseState.Disabled)
        {
            return;
        }

        switch (_offenseState)
        {
            case PlayerOffenseState.Attacking:
                PerformAttack();
                break;
            case PlayerOffenseState.Shooting:
                PerformShooting();
                break;
        }
    }

    public void DisableOffense()
    {
        _offenseState = PlayerOffenseState.Disabled;
    }

    public void EnableOffense()
    {
        _offenseState = PlayerOffenseState.Neutral;
    }

    private void OnEnable()
    {
        _input.OnAttack += () => _attackPressed = true;
        _input.OnShoot += () => _shootPressed = true;
    }

    private void OnDisable()
    {
        _input.OnAttack -= () => _attackPressed = true;
        _input.OnShoot -= () => _shootPressed = true;
    }

    private void ResetFrameInputs()
    {
        _attackPressed = false;
        _shootPressed = false;
    }

    // ============== Player Attacking ==============
    private void HandleAttack()
    {
        if (!_attackPressed || !canAttack || _isShooting || _playerMovement.GetIsCrouched() || !_playerMovement.GetIsGrounded())
        {
            return;
        }
        _offenseState = PlayerOffenseState.Attacking;
        canAttack = false;
        _isAttacking = true;
        _attackTimeLeft = _attackDuration;
    }

    private void HandleAttackCooldowns()
    {
        if (canAttack)
        {
            return;
        }

        _attackCooldownTimer += Time.deltaTime;

        if (_attackCooldownTimer >= _attackCooldown)
        {
            _attackCooldownTimer = 0;
            canAttack = true;
        }
    }

    private void PerformAttack()
    {

        _attackArmPrefab.GetComponent<Collider>().enabled = true;
        // Play Punch Animation
        // PLayer Punch Sound

        FinishAttack();
    }

    private void FinishAttack()
    {
        _attackTimeLeft -= Time.fixedDeltaTime;
        if (_attackTimeLeft <= 0)
        {
            _attackArmPrefab.GetComponent<Collider>().enabled = false;
            _offenseState = PlayerOffenseState.Neutral;
            _isAttacking = false;
        }
    }

    // ============== Player Attacking ==============
    private void HandleShooting()
    {
        if (!_shootPressed || !canShoot || _isAttacking || _playerMovement.GetIsCrouched() || !_playerMovement.GetIsGrounded())
        {
            return;
        }
        _offenseState = PlayerOffenseState.Shooting;
        canShoot = false;
        _isShooting = true;
    }

    private void HandleShootCooldowns()
    {
        if (canShoot)
        {
            return;
        }

        _shootCooldownTimer += Time.deltaTime;

        if (_shootCooldownTimer >= _shootCooldown)
        {
            _shootCooldownTimer = 0;
            canShoot = true;
            _isShooting = false;
        }
    }

    private void PerformShooting()
    {
        Vector3 SpawnPoint = transform.position + transform.forward * _shotPointForwardOffset + transform.up * _shotPointUpOffset + transform.right * _shotPointRightOffset;
        GameObject Shot = Instantiate(_shotPrefab, SpawnPoint, Quaternion.identity);

        if (Shot.GetComponent<Rigidbody>() != null)
        {
            Shot.GetComponent<Rigidbody>().linearVelocity = transform.forward * _shootForce;
        }
        else
        {
            Debug.LogError("Shot Has No RigidBody");
        }
        _offenseState = PlayerOffenseState.Neutral;
        // Play Shoot Animation
        // PLayer Shoot Sound

    }

    private void OnDrawGizmos()
    {
        //Shoot From Point
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * _shotPointForwardOffset + transform.up * _shotPointUpOffset + transform.right * _shotPointRightOffset, 0.1f);
    }
}
