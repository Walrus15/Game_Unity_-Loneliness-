using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DogEnemyAI : MonoBehaviour
{
    public enum DogState
    {
        Idle,
        Chase,
        Attack
    }

    [Header("Настройки состояний")]
    public DogState currentState = DogState.Idle;

    [Header("Настройки игрока")]
    public Transform playerTransform;
    public string playerTag = "Player";

    [Header("Настройки обнаружения и атаки")]
    public float detectionRadius = 10f;
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public float attackCooldown = 1.0f;
    private bool canAttack = true;

    [Header("Настройки движения")]
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 5f;

    [Header("Объекты и компоненты собаки")]
    public NavMeshAgent agent;
    public Animator animator;

    private float distanceToPlayer;

    void Start()
    {
        // Проверка NavMesh
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                enabled = false;
                return;
            }
        }
        agent.speed = moveSpeed;

        // Проверка Animator
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator не найден");
            }
        }

        // Поиск игрока по тэгу
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                enabled = false;
                return;
            }
        }

        SetState(DogState.Idle);
    }

    void Update()
    {
        if (playerTransform == null) return;

        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Логика состояний
        switch (currentState)
        {
            case DogState.Idle:
                if (distanceToPlayer <= detectionRadius)
                {
                    SetState(DogState.Chase);
                }
                break;

            case DogState.Chase:
                if (distanceToPlayer > detectionRadius)
                {
                    SetState(DogState.Idle);
                }
                else if (distanceToPlayer <= attackRange)
                {
                    SetState(DogState.Attack);
                }
                else
                {
                    ChasePlayer();
                }
                break;

            case DogState.Attack:
                if (distanceToPlayer > attackRange && distanceToPlayer <= detectionRadius)
                {
                    SetState(DogState.Chase);
                }
                else if (distanceToPlayer > detectionRadius)
                {
                    SetState(DogState.Idle);
                }
                else
                {
                    PerformAttack();
                }
                break;
        }

        UpdateAnimator();
    }

    void SetState(DogState newState)
    {
        if (currentState == newState) return; // Избегаем повторной установки того же состояния

        currentState = newState;
        Debug.Log("Собака перешла в состояние: " + currentState);

        switch (currentState)
        {
            case DogState.Idle:
                if (agent.enabled) agent.isStopped = true;
                break;
            case DogState.Chase:
                if (agent.enabled) agent.isStopped = false;
                break;
            case DogState.Attack:
                if (agent.enabled) agent.isStopped = true;
                break;
        }
    }

    void ChasePlayer()
    {
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        RotateTowardsTarget(playerTransform.position);
    }

    void PerformAttack()
    {
        RotateTowardsTarget(playerTransform.position);

        if (canAttack)
        {
            PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log("Собака атакует игрока! Наносит " + attackDamage + " урона.");
            }
            else
            {
                Debug.LogWarning("У игрока нет компонента PlayerHealth!");
            }
            StartCoroutine(AttackCooldownRoutine());
        }
    }

    void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    IEnumerator AttackCooldownRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void UpdateAnimator()
    {
        if (animator != null)
        {
            float currentSpeed = agent.velocity.magnitude / agent.speed;
            if (currentState == DogState.Idle || currentState == DogState.Attack)
            {
                currentSpeed = 0;
            }
            animator.SetFloat("Speed", currentSpeed);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}