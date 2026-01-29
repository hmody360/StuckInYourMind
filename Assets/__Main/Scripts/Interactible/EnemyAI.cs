using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamageable
{
    // Patrol points
    [Header("Patrol Points")]
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    // Player reference
    [Header("Player Reference")]
    private Transform _player;
    [Header("Movement Modfiers")]
    // Movement speeds
    [SerializeField] private float _patrolSpeed = 2f;
    [SerializeField] private float _chaseSpeed = 4f;
    // Distances
    [SerializeField] private float _chaseRange = 8f;
    [SerializeField] private float _chaseLoseRange = 15f;
    // Vision
    [SerializeField] private float _viewAngle = 45f;
    // Enemy health
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private float _currentHealth;
    [SerializeField] private bool _isDead = false;
    // Current patrol target
    [Header("Current Target")]
    [SerializeField] private Transform _currentTarget;
    private NavMeshAgent _agent;
    // Audio sources
    [Header("SFX Sources")]
    [SerializeField] private AudioSource _WalkSFX;
    [SerializeField] private AudioSource _DetectionSFX;
    [SerializeField] private AudioSource _PatrolSFX;
    [SerializeField] private AudioSource _ChaseSFX;
    [SerializeField] private AudioSource _DamageSFX;
    // Attack parameters
    [Header("Attack Modfiers")]
    public float attackRange = 3f;
    public float attackDamage = 10f;
    public float attackCooldown = 3f;
    [SerializeField] float Timer = 0f;
    //Animator
    private Animator _enemyAnimator;
    private Rigidbody _enemyRB;
    private WinChecker _winChecker;
    private EnemyUI _enemyUI;

    [SerializeField] private AudioClip[] _damageAudioClips;

    bool isChasing = false;
   
    bool DetectPlayed = false;

    private void Awake()
    {
        _enemyAnimator = GetComponent<Animator>();
        _enemyRB = GetComponent<Rigidbody>();
        _enemyUI = GetComponentInChildren<EnemyUI>();
    }
    void Start()
    {
        _enemyRB.freezeRotation = true;
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _winChecker = GameObject.FindGameObjectWithTag("WinChecker").GetComponent<WinChecker>();

        // Enemy starts patrolling towards point A
        _currentTarget = _pointA;
        // Set enemy health to full at the start
        _currentHealth = _maxHealth;
        // Get NavMeshAgent for movement
        _agent = GetComponent<NavMeshAgent>();
        
        if (_agent != null)
        {
            _agent.updateRotation = false;
            _agent.speed = _patrolSpeed;
            _agent.stoppingDistance = 0.5f;
            _agent.SetDestination(_currentTarget.position);
        }
        else
        {
            Debug.LogError(" NavMeshAgent is missing on this enemy");
        }

        _enemyUI.UpdateHealthSlider(_currentHealth, _maxHealth);
    }
    void Update()
    {
        if (_isDead) return;


        Timer += Time.deltaTime;
        // prevent AI logic from running if agent or player references is missing
        if (_agent == null || _player == null)
            return;

        Vector3 dirToPlayer = _player.position - transform.position;
        float distToPlayer = dirToPlayer.magnitude;
        bool canSeePlayer = false;
        // patrol or chase logic
        if (!isChasing)
        {
            if (distToPlayer <= _chaseRange)
            {
                Vector3 flatDir = dirToPlayer;
                flatDir.y = 0f;
                float angle = Vector3.Angle(transform.forward, flatDir);
                if (angle <= _viewAngle * 0.5f)
                {
                    RaycastHit hit;
                    Vector3 eyePos = transform.position + Vector3.up * 0.5f;
                    if (Physics.Raycast(eyePos, flatDir.normalized, out hit, _chaseRange))
                    {
                        if (hit.transform == _player)
                        {
                            canSeePlayer = true;
                            Debug.Log("I'm following player");
                        }
                    }
                }
            }
            // if player is visible, begin chase
            if (canSeePlayer)
            {
                if (!DetectPlayed && _DetectionSFX != null)
                {
                    _DetectionSFX.Play();
                    DetectPlayed = true;
                }
                isChasing = true;
            }
        }
        // chase mode 
        else
        {
            if (distToPlayer > _chaseLoseRange)
            {
                isChasing = false;
                DetectPlayed = false; // detect sound can play again next time
                _agent.speed = _patrolSpeed;
                _enemyUI.HideHealthSlider();
                _agent.SetDestination(_currentTarget.position);// resume patrolling btween points A and B
            }
        }
        // execute state
        if (isChasing)
        {
            ChasePlayer();// run chase behavior
        }
        else
        {
            Patrol();// run patrol behavior
        }
        //attack player
        if (distToPlayer <= attackRange && Timer >= attackCooldown && isChasing)
        {
            _enemyAnimator.SetTrigger("HitTrigger");
            _player.GetComponent<IDamageable>().takeDamage(attackDamage);
            Timer = 0;
        }

        //Animator values updating
        _enemyAnimator.SetBool("isStopped", _agent.isStopped);
        _enemyAnimator.SetBool("isChasing", isChasing);
    }
    // patrol behavior
    void Patrol()
    {
        _agent.stoppingDistance = 0.5f;
        // check if the agent has reached its destination
        if (_agent.pathPending)
            return;
        // if close enough to the target, switch to the other point
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _currentTarget = (_currentTarget == _pointA) ? _pointB : _pointA;

            _agent.speed = _patrolSpeed;
            _agent.SetDestination(_currentTarget.position);
        }
        // smoothly rotate enymy to face movement direction
        LookAt(_currentTarget.position);
        //sound effects
        if (!_WalkSFX.isPlaying)
        {
            _WalkSFX.Play();
        }
        if (!_PatrolSFX.isPlaying)
        {
            _PatrolSFX.Play();
        }
        if (_ChaseSFX.isPlaying)
        {
            _ChaseSFX.Stop();
        }
    }
    // chase behavior
    void ChasePlayer()
    {
        // create a target position with Y fixed (to avoid vertical rotation)
        Vector3 targetPos = new Vector3(_player.position.x, transform.position.y, _player.position.z);
        _agent.speed = _chaseSpeed;
        _agent.stoppingDistance = attackRange;
        _agent.SetDestination(targetPos);
        // rotate to face the player
        LookAt(_player.position);

        //Stop when the player has been reached
        if(_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _agent.isStopped = true;
        }
        else
        {
            _agent.isStopped = false;
        }

        // sound effects
        if (!_WalkSFX.isPlaying)
        {
            _WalkSFX.Play();
        }
        if (!_ChaseSFX.isPlaying)
        {
            _ChaseSFX.Play();
        }
        if (_PatrolSFX.isPlaying)
        {
            _PatrolSFX.Stop();
        }
    }
             // look rotation
            void LookAt(Vector3 targetPos)
            {
                // get direction vector
                Vector3 dir = targetPos - transform.position;
                dir.y = 0;
                if (dir.sqrMagnitude > 0.001f)
                {
                    // calculate desired rotation
                    Quaternion rot = Quaternion.LookRotation(dir);
                    // smoothly rotate towards the target
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }
    }
    // method to apply damage to the enemy
    public void takeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            onDeath();
        }
        else
        {
            _enemyAnimator.SetTrigger("KnockbackTrigger");
            _DamageSFX.PlayOneShot(_damageAudioClips[0]);
        }
        _enemyUI.ShowHealthSlider();
        _enemyUI.UpdateHealthSlider(_currentHealth, _maxHealth);

        if (!isChasing)
        {
            isChasing = true;
        }
    }
    public void onDeath()
    {
        if(_isDead) return;

        if(_winChecker != null)
        {
            GameObject self = _winChecker._EnemiesList.Find(item => item == gameObject);
            if (self != null)
            {
                _winChecker._EnemiesList.Remove(self);
            }
        }
        
        // remove enemy from the scene & stop all actions
        _agent.isStopped = true;
        _agent.enabled = false;
        _enemyAnimator.SetTrigger("DeathTrigger");
        _WalkSFX.Stop();
        _DetectionSFX.Stop();
        _ChaseSFX.Stop();
        _PatrolSFX.Stop();
        _DamageSFX.PlayOneShot(_damageAudioClips[1]);
        _isDead = true;
        Destroy(transform.parent.gameObject, 2f);
    }

    // debug gizmos
    void OnDrawGizmos()
    {
        if (_player == null)
            return;
        Vector3 eyePos = transform.position + Vector3.up * 0.5f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, _chaseLoseRange);

        Gizmos.DrawRay(eyePos, transform.forward);

        float halfAngle = _viewAngle * 0.5f;

        Vector3 leftDir = Quaternion.Euler(0, -halfAngle, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, halfAngle, 0) * transform.forward;
         
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(eyePos, leftDir * _chaseRange);
        Gizmos.DrawRay(eyePos, rightDir * _chaseRange);

        Gizmos.color = Color.red;
        Vector3 flatDir = _player.position - transform.position;
        flatDir.y = 0f;

        Gizmos.DrawRay(eyePos, flatDir.normalized * _chaseRange);
    }
}

