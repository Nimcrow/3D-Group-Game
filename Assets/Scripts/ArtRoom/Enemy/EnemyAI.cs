using UnityEngine;

public class AdaptiveEnemyAI : MonoBehaviour
{
    // References
    [SerializeField] private GameObject playerObject; // Player reference
    private Transform player;

    // Layers
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;

    // Health
    [SerializeField] private float health = 100f;

    // Patroling
    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float patrolSpeed = 2f;
    private Vector3 patrolTarget;
    private bool patrolTargetSet;

    // Combat
    [Header("Combat Settings")]
    [SerializeField] private float attackIntervalMin = 1.5f;
    [SerializeField] private float attackIntervalMax = 3.5f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20f;
    private bool alreadyAttacked;

    // Detection
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 5f;
    private bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        Debug.Log("Enemy AI initialized and ready.");
        ScheduleRandomShoot();
    }

    private void Update()
    {
        if (player == null)
        {
            Patrol();
            return;
        }

        CheckPlayerState();

        if (!playerInSightRange && !playerInAttackRange)
            Patrol();
        else if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        else if (playerInAttackRange && playerInSightRange)
            AttackPlayer();
    }

    private void CheckPlayerState()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, detectionRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
    }

    private void Patrol()
    {
        if (!patrolTargetSet)
            SetRandomPatrolTarget();

        transform.position = Vector3.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, patrolTarget) < 1f)
            patrolTargetSet = false;
    }

    private void SetRandomPatrolTarget()
    {
        float randomX = Random.Range(-patrolRadius, patrolRadius);
        float randomZ = Random.Range(-patrolRadius, patrolRadius);
        patrolTarget = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(patrolTarget, Vector3.down, 2f, groundLayer))
            patrolTargetSet = true;
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * patrolSpeed * Time.deltaTime, Space.World);
    }

    private void AttackPlayer()
    {
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ShootProjectile(player.position);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), Random.Range(attackIntervalMin, attackIntervalMax));
        }
    }

    private void ShootProjectile(Vector3 targetPosition)
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward * 1.5f, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = (targetPosition - transform.position).normalized * projectileSpeed;
    }

    private void ScheduleRandomShoot()
    {
        float randomInterval = Random.Range(attackIntervalMin, attackIntervalMax);
        Invoke(nameof(ShootRandomProjectile), randomInterval);
    }

    private void ShootRandomProjectile()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.2f, 1f), Random.Range(-1f, 1f)).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position + randomDirection * 1.5f, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = randomDirection * projectileSpeed;

        ScheduleRandomShoot();
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
