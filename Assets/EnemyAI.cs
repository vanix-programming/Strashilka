using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 2f;

    [Header("Detection")]
    [SerializeField] private float chaseDistance = 10f;
    [SerializeField] private float attackDistance = 1.8f;

    [Header("Speeds")]
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3.5f;

    [Header("Animator Params")]
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string isChasingParam = "IsChasing";
    [SerializeField] private string isAttackingParam = "IsAttacking";

    private int currentPatrolIndex;
    private float waitTimer;
    private bool isWaiting;

    private enum ZombieState
    {
        Patrol,
        Chase,
        Attack
    }

    private ZombieState currentState;

    private void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        currentState = ZombieState.Patrol;

        if (patrolPoints.Length > 0)
        {
            GoToNextPatrolPoint();
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Визначаємо стан
        if (distanceToPlayer <= attackDistance)
        {
            currentState = ZombieState.Attack;
        }
        else if (distanceToPlayer <= chaseDistance)
        {
            currentState = ZombieState.Chase;
        }
        else
        {
            currentState = ZombieState.Patrol;
        }

        // Поведінка по станах
        switch (currentState)
        {
            case ZombieState.Patrol:
                Patrol();
                break;

            case ZombieState.Chase:
                ChasePlayer();
                break;

            case ZombieState.Attack:
                AttackPlayer();
                break;
        }

        UpdateAnimations();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            agent.isStopped = true;
            return;
        }

        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0f;

        if (isWaiting)
        {
            agent.isStopped = true;
            waitTimer += Time.deltaTime;

            if (waitTimer >= patrolWaitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                GoToNextPatrolPoint();
            }

            return;
        }

        agent.isStopped = false;

        if (!agent.pathPending && agent.remainingDistance <= 0.2f)
        {
            isWaiting = true;
        }
    }

    private void ChasePlayer()
    {
        isWaiting = false;
        waitTimer = 0f;

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = attackDistance - 0.1f;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        isWaiting = false;
        waitTimer = 0f;

        agent.isStopped = true;

        // Повертаємо зомбі до гравця
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0f;

        if (lookPos != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8f);
        }

        // Тут потім можна буде додати нанесення урону
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.isStopped = false;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        currentPatrolIndex++;
        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;
    }

    private void UpdateAnimations()
    {
        float speed = agent.velocity.magnitude;

        // float параметр швидкості
        animator.SetFloat("Speed", speed);

        // bool параметри
        animator.SetBool(isChasingParam, currentState == ZombieState.Chase);
        animator.SetBool(isAttackingParam, currentState == ZombieState.Attack);
    }

    private void OnDrawGizmosSelected()
    {
        // Дистанція агру
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        // Дистанція атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}