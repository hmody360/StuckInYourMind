using UnityEngine;
using static GameEnums;
public class PlayerAttack : MonoBehaviour
{
    [Header("General Modfiers")]
    public bool canAttack = true;
    public bool canShoot = true;
    private PlayerOffenseState _offenseState = PlayerOffenseState.Neutral;

    [Header("Attack Modfiers")]
    [SerializeField] private GameObject _attackArmPrefab;
    private Collider _armCollider;
    [SerializeField] private float _attackTimeLeft;
    [SerializeField] private float _attackDuration = 0.15f;
    [SerializeField] private float _attackCooldown = 3f;
    [SerializeField] private float _attackCooldownTimer;
    [SerializeField] private float _attackDamage = 5f;


    [Header("Shoot Modfiers")]
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

    //Components
    //private Animator _animator;
    private PlayerInputHandler _input;
    private PlayerMovement _playerMovement;
    private Transform _cameraTransform;

    private void Awake()
    {
        //_animator = GetComponent<Animator>();
        _input = GetComponent<PlayerInputHandler>();
        _playerMovement = GetComponent<PlayerMovement>();
        _cameraTransform = Camera.main.transform;

        if (_attackArmPrefab != null)
        {
            if (_attackArmPrefab.TryGetComponent(out AttackImpact attackImpact))
            {
                attackImpact.attackPower = _attackDamage;
            }

            _attackArmPrefab.TryGetComponent(out _armCollider);
                if(_armCollider != null)
            {
                _armCollider.enabled = false;
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Update()
    {
        if (_offenseState == PlayerOffenseState.Disabled)
        {
            return;
        }

        HandleCooldowns();
    }

    private void FixedUpdate()
    {
        if (_offenseState == PlayerOffenseState.Attacking)
        {
            UpdateAttackTimer();
        }
    }

    public void DisableOffense()
    {
        _offenseState = PlayerOffenseState.Disabled;
        _armCollider.enabled = false;
    }

    public void EnableOffense()
    {
        _offenseState = PlayerOffenseState.Neutral;
    }

    private void OnEnable()
    {
        _input.OnAttack += TryStartAttack;
        _input.OnShoot += TryStartShoot;
    }

    private void OnDisable()
    {
        _input.OnAttack -= TryStartAttack;
        _input.OnShoot -= TryStartShoot;
    }

    // ============== Player Attacking ==============
    private void TryStartAttack()
    {
        if (!canAttack || _offenseState != PlayerOffenseState.Neutral || _playerMovement.GetIsCrouched() || !_playerMovement.GetIsGrounded())
        {
            return;
        }
        _offenseState = PlayerOffenseState.Attacking;
        canAttack = false;
        _attackTimeLeft = _attackDuration;
        _armCollider.enabled = true;
        // Play Punch Animation
        // PLayer Punch Sound
    }

    private void HandleCooldowns()
    {
        if (!canAttack)
        {
            _attackCooldownTimer += Time.deltaTime;

            if (_attackCooldownTimer >= _attackCooldown)
            {
                _attackCooldownTimer = 0;
                canAttack = true;
            }
        }

        if (!canShoot)
        {
            _shootCooldownTimer += Time.deltaTime;

            if (_shootCooldownTimer >= _shootCooldown)
            {
                _shootCooldownTimer = 0;
                canShoot = true;
            }
        }
        

    }

    private void UpdateAttackTimer()
    {
        _attackTimeLeft -= Time.fixedDeltaTime;
        if (_attackTimeLeft <= 0)
        {
            _armCollider.enabled = false;
            _offenseState = PlayerOffenseState.Neutral;
        }
    }

    // ============== Player Attacking ==============
    private void TryStartShoot()
    {
        if (!canShoot || _offenseState != PlayerOffenseState.Neutral || _playerMovement.GetIsCrouched() || !_playerMovement.GetIsGrounded())
        {
            return;
        }
        _offenseState = PlayerOffenseState.Shooting;
        canShoot = false;

        // Change Player Rotation On Shot
        Vector3 lookDir = _cameraTransform.forward;
        lookDir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(lookDir);
        transform.rotation = targetRotation;
        _playerMovement.SetTargetRotation(targetRotation);

        FireShot();

        _offenseState = PlayerOffenseState.Neutral;
    }

    private void FireShot()
    {
        Vector3 SpawnPoint = transform.position + transform.forward * _shotPointForwardOffset + transform.up * _shotPointUpOffset + transform.right * _shotPointRightOffset;
        GameObject Shot = Instantiate(_shotPrefab, SpawnPoint, Quaternion.identity);

        if (Shot.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = _cameraTransform.forward * _shootForce;
        }
        else
        {
            Debug.LogError("Shot Has No RigidBody");
        }
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
