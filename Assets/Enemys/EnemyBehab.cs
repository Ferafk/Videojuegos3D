using System.Collections;
using System.Collections.Generic;
using Gaming.FinalCharacterController;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyState
{
    EnteringLevel,
    SearchingBoxes,
    AttackingBox,
    Stunned,
    ChasingPlayer,
    Idle
}

public class EnemyBehab : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float detectionRadius = 5f;

    [Header("Combat Settings")]
    [SerializeField] private int boxDamage = 1;
    [SerializeField] private int playerDamage = 1;
    [SerializeField] private float stunDuration = 10f;
    [SerializeField] private float attackCooldown = 0.7f;
    private float _nextAttackTime = 0f;


    private EnemyState _currentState = EnemyState.EnteringLevel;
    private Transform _door;
    private Transform _targetBox;
    private Transform _player;

    private int _shootCount = 0;
    private float _currentHealth = 100f;
    private bool _isStunned = false;
    private Animator _animator;


    private void Awake()
    {
        FindInitialDoor();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
         switch (_currentState)
        {
            case EnemyState.EnteringLevel:
                MoveTowardsDoor();
                break;
            case EnemyState.SearchingBoxes:
                FindNearestBox();
                break;
            case EnemyState.AttackingBox:
                AttackCurrentBox();
                break;
            case EnemyState.Stunned:
                break;
            case EnemyState.ChasingPlayer:
                ChaseAndAttackPlayer();
                break;
        }

        CheckPlayerProximity();
    }

    private void FindInitialDoor()
    {
        _door = GameObject.FindGameObjectWithTag("LevelEntryDoor")?.transform;
        if (_door == null)
        {
            Debug.LogError("No entry door found in the scene!");
        }
    }

    private void MoveTowardsDoor()
    {
        if (_door == null) return;

        _animator.SetBool("Move", true);
        Vector3 targetPosition = _door.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        LookAtMovementDirection(targetPosition);

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            Debug.Log("Entra enemigo");
            _currentState = EnemyState.SearchingBoxes;
        }
    }

    private void FindNearestBox()
    {
        Collider[] boxes = Physics.OverlapSphere(transform.position, detectionRadius, LayerMask.GetMask("Boxes"));

        if (boxes.Length > 0)
        {
            _targetBox = GetClosestBox(boxes);
            Debug.Log("Llendo a caja");
            _currentState = EnemyState.AttackingBox;
        }
    }

    private Transform GetClosestBox(Collider[] boxes)
    {
        Transform closestBox = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider box in boxes)
        {
            float distance = Vector3.Distance(transform.position, box.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestBox = box.transform;
            }
        }

        return closestBox;
    }

    private void AttackCurrentBox()
    {
        if (_targetBox == null)
        {
            _currentState = EnemyState.SearchingBoxes;
            return;
        }

        _animator.SetBool("Move", true);
        _animator.SetBool("Attack", false);

        Vector3 targetPosition = _targetBox.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        LookAtMovementDirection(targetPosition);

        if (Vector3.Distance(transform.position, _targetBox.position) <= attackRange)
        {

            if (Time.time >= _nextAttackTime)
            {
                Breakable damageable = _targetBox.GetComponent<Breakable>();

                if (damageable != null)
                {
                    _animator.SetBool("Move", false);
                    _animator.SetBool("Attack", true);
                    damageable.Damage(boxDamage);

                    // Establecer el próximo tiempo de ataque
                    _nextAttackTime = Time.time + attackCooldown;

                    // Verificar si la caja fue destruida
                    if (damageable.IsDestroyed())
                    {
                        _currentState = EnemyState.SearchingBoxes;
                        _targetBox = null;
                    }
                    // Si no está destruida, seguir en estado de ataque
                    else
                    {
                        _currentState = EnemyState.AttackingBox;
                    }
                }
            }
        }
    }

    private void CheckPlayerProximity()
    {
        if (_player == null) return;

        PlayerController playerController = _player.GetComponent<PlayerController>();

        if (playerController == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            HandlePlayerDetection(playerController);
        }
    }

    private void HandlePlayerDetection(PlayerController playerController)
    {
        if (!playerController.tieneComida)
        {
            HandleHungryPlayerInteraction(playerController);
        }
        else
        {
            HandleFedPlayerInteraction(playerController);
        }
    }

    private void HandleHungryPlayerInteraction(PlayerController playerController)
    {
        if (Vector3.Distance(transform.position, _player.position) < attackRange)
        {
            playerController.TakeDamage(playerDamage);
            _currentState = EnemyState.SearchingBoxes;
        }
    }

    private void HandleFedPlayerInteraction(PlayerController playerController)
    {
        _currentState = EnemyState.ChasingPlayer;
    }

    private void ChaseAndAttackPlayer()
    {
        if (_player == null) return;

        Vector3 targetPosition = _player.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        LookAtMovementDirection(targetPosition);

        _animator.SetBool("Move", true);
        _animator.SetBool("Attack", false);

        if (Vector3.Distance(transform.position, _player.position) <= attackRange)
        {
            PushPlayer();
        }
    }

    private void PushPlayer()
    {
        if (_player == null) return;

        _animator.SetBool("Move", false);
        _animator.SetBool("Attack", true);
        CharacterController playerController = _player.GetComponent<CharacterController>();
        if (playerController != null)
        {
            Vector3 pushDirection = (_player.position - transform.position).normalized;
            Vector3 pushVector = pushDirection * 2f;

            // Añadir un pequeño componente vertical para un empuje más natural
            pushVector.y = 1f;

            playerController.Move(pushVector);
        }
    }

    public void ReceiveDamage(float damageAmount, bool hasFood)
    {
        if (hasFood)
        {
            _currentHealth -= damageAmount;
            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        else
        {
            _shootCount++;
            if (_shootCount >= 3)
            {
                StartCoroutine(StunRoutine());
            }
        }
    }

    private IEnumerator StunRoutine()
    {
        _isStunned = true;
        _currentState = EnemyState.Stunned;
        yield return new WaitForSeconds(stunDuration);
        _isStunned = false;
        _shootCount = 0;
        _currentState = EnemyState.SearchingBoxes;
    }

    private void LookAtMovementDirection(Vector3 targetPosition)
    {
        if (targetPosition != transform.position)
        {
            Vector3 lookDirection = (targetPosition - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

}
