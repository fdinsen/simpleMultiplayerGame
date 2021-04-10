using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHandler : MonoBehaviour {
    [SerializeField] private double _lowerDamageMultiplier = 0.02f;
    [SerializeField] private double _upperDamageMultiplier = 0.05f;
    [SerializeField] private AudioClip _zombieClip;
    [SerializeField] private AudioClip _hurtClip;
    [SerializeField] private AudioClip _deathClip;

    private float _speed;
    private Transform _target;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private Rigidbody[] _rigidbodies;
    private int _health;
    private BoxCollider _collider;

    private bool _destroy = false; // Used if "Start" failed

    private RoundHandler _roundHandler;
    private PlayerHealth _player;

    private float spawnTimeout = 1;
    private float countdownToSound = 10;
    private const double _attackDistance = 0.5;


    // Start is called before the first frame update
    public void Start()
    {
        SetClassParameters();
        CheckClassParameters();

        if (_destroy)
        {
            Destroy(gameObject);
        }

        foreach (Rigidbody rb in _rigidbodies)
        {
            if (rb.gameObject != gameObject)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }

        GoRagdoll(false);

        _speed = Random.Range(0.2F, 5F);
        _navMeshAgent.speed = _speed;
        _animator.SetFloat("Speed", _speed);

        _navMeshAgent.SetDestination(_target.position);
    }

    // FixedUpdate is called 50 times per second
    private void FixedUpdate()
    {
        if (IsAlive())
        {
            Move();
        }
        DecrementSpawnTimeout();
        PlayAudioClip();
    }

    /* ##################################################

           Helper Methods

   ################################################## */

    private void SetClassParameters()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _navMeshAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        _animator = this.GetComponent<Animator>();
        _rigidbodies = GetComponentsInChildren<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
        _roundHandler = FindObjectOfType<RoundHandler>();
        _player = FindObjectOfType<PlayerHealth>();
        _health = 50;
    }

    private void CheckClassParameters()
    {
        if (_target == null)
        {
            Debug.LogError(gameObject.name + " couldn't locate a player!");
            _destroy = true;
        }

        if (_navMeshAgent == null)
        {
            Debug.LogError(gameObject.name + " is missing a NavMeshAgent component!");
            _destroy = true;
        }

        if (_animator == null)
        {
            Debug.LogError(gameObject.name + " is missing a Animator component!");
            _destroy = true;
        }

        if (_zombieClip == null)
        {
            Debug.LogError("No audioclip set for " + gameObject.name);
        }

        if(_hurtClip == null)
        {
            Debug.LogError("No hurt audioclip set for " + gameObject.name);
        }
        if(_deathClip == null)
        {
            Debug.LogError("No death audioclip set for " + gameObject.name);
        }
    }

    private void DecrementSpawnTimeout()
    {
        if (spawnTimeout >= 0)
        {
            spawnTimeout -= Time.deltaTime;
        }
    }

    private void PlayAudioClip()
    {
        if (IsAlive())
        {
            if (_zombieClip != null && countdownToSound <= 0)
            {
                AudioSource.PlayClipAtPoint(_zombieClip, gameObject.transform.position);
            }
            if (countdownToSound > 0)
            {
                countdownToSound -= Random.Range(0.001f, 0.5f);
            }
            else
            {
                countdownToSound = Random.Range(20, 300);
            }
        }
    }

    /* ##################################################

            Health related

    ################################################## */

    public void SetHealth(int health) {
        _health = health;
    }

    public void Damage(int damage) {
        if(_health > 0) {
            AudioSource.PlayClipAtPoint(_hurtClip, gameObject.transform.position);
            _health -= damage;
            CheckHealth();
        }
    }

    private void CheckHealth() {
        if(_health <= 0) {
            Die();
        }
    }

    private void Die() {
        AudioSource.PlayClipAtPoint(_deathClip, gameObject.transform.position);
        _roundHandler.CheckZombiesLeft();
        _collider.enabled = false;
        _animator.enabled = false;
        _navMeshAgent.speed = 0;
        GoRagdoll(true);
    }

    public bool IsAlive() {
        return _health > 0;
    }

    private void GoRagdoll(bool status) {
        _rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in _rigidbodies) {
            if (rb.gameObject != gameObject) {
                rb.useGravity = status;
                rb.isKinematic = !status;
            }
        }
    }

    /* ##################################################

            Movement related

    ################################################## */

    private float _distance;
    private bool _notAttacking;

    private void Move() {
        float stoppingDistance = _navMeshAgent.stoppingDistance;
        _navMeshAgent.SetDestination(new Vector3(_target.position.x, 0, _target.position.z));

        _distance = _navMeshAgent.GetPathRemainingDistance() - stoppingDistance;
        _notAttacking = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Attack";
        if(_distance <= _attackDistance && _distance != (-1 - stoppingDistance) && spawnTimeout <= 0) {
            DoAttack(_notAttacking);
        } else if(_distance > 0 && _notAttacking) {
            _animator.SetFloat("Speed", _speed);
            _navMeshAgent.speed = _speed;
        }
    }

    private void DoAttack(bool notAttacting) {
        if(_navMeshAgent.speed != 0) {
            _navMeshAgent.speed = 0;
            _animator.SetFloat("Speed", 0);
        }

        if(notAttacting) {
            _animator.SetTrigger("Attack");
        }

        if(_isAttacking) {
            int minDamage = System.Convert.ToInt32(_health * _lowerDamageMultiplier);
            int maxDamage = System.Convert.ToInt32(_health * _upperDamageMultiplier);
            int damage = Random.Range(minDamage, maxDamage);
            _player.Damage(damage);
        }
    }

    /* ##################################################

            Attack related

    ################################################## */

    private bool _isAttacking = false;

    public void StartAttackEvent() {
        _isAttacking = true;
    }

    public void StopAttackEvent() {
        _isAttacking = false;
    }

    /* ##################################################

            Victory related

    ################################################## */

    public void StartDancing() {
        _navMeshAgent.speed = 0;
        _animator.SetTrigger("Victory");
    }

}
