using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject playerObject; // Drag the player GameObject here in the Inspector
    private Transform player;       // Reference to the player's Transform

    public LayerMask whatIsGround, whatIsPlayer;

    public float health = 100;

    // Patrolling
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks = 1f;
    private bool alreadyAttacked;
    public GameObject projectile; // Regular projectile
    public GameObject specialProjectile; // Special projectile
    private int attackCounter = 0; // Counter to track attacks

    // States
    public float sightRange = 10f, attackRange = 5f;
    private bool playerInSightRange, playerInAttackRange;

    // Movement
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f; // Speed for chasing

    // Shield
    public GameObject shield; // Reference to the shield GameObject
    public int maxHitsToActivateShield = 4;
    private int currentHits = 0;
    private bool isShieldActive = false;
    private MeshRenderer shieldRenderer;
    private SphereCollider shieldCollider;

    private void Awake()
    {
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // Ensure the shield is deactivated at the start
        if (shield != null)
        {
            shieldRenderer = shield.GetComponent<MeshRenderer>();
            shieldCollider = shield.GetComponent<SphereCollider>();

            if (shieldRenderer != null) shieldRenderer.enabled = false;
            if (shieldCollider != null) shieldCollider.enabled = false;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling(); // Patrol when no player is detected
        if (playerInSightRange && !playerInAttackRange) ChasePlayer(); // Chase when player is detected but out of attack range
        if (playerInAttackRange && playerInSightRange) AttackPlayer(); // Attack when in attack range

        // Test Shield Activation with "E" Key
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed, registering as hit.");
            TakeDamage(0); // Simulates a hit without reducing health
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            Vector3 direction = (walkPoint - transform.position).normalized;
            transform.Translate(direction * walkSpeed * Time.deltaTime, Space.World);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            // Walk point reached
            if (distanceToWalkPoint.magnitude < 1f)
                walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * chaseSpeed * Time.deltaTime, Space.World);
    }

    private void AttackPlayer()
    {
        // Look at the player
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            attackCounter++;

            // Determine which projectile to shoot
            GameObject chosenProjectile = (attackCounter % 4 == 0) ? specialProjectile : projectile;

            // Adjusted spawn position to shoot lower
            Vector3 spawnPosition = transform.position + transform.forward * 1f + transform.up * 0.5f; // Lowered vertical offset

            if (chosenProjectile != null)
            {
                Rigidbody rb = Instantiate(chosenProjectile, spawnPosition, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                rb.AddForce(transform.up * 4f, ForceMode.Impulse); // Reduced upward force for a lower trajectory
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        if (isShieldActive)
        {
            Debug.Log("Attack blocked by shield!");
            DeactivateShield();
            return;
        }

        currentHits++;
        Debug.Log($"Enemy hit! Current hits: {currentHits}/{maxHitsToActivateShield}");

        if (currentHits >= maxHitsToActivateShield)
        {
            Debug.Log("Shield activation triggered!");
            ActivateShield();
        }
        else
        {
            Debug.Log($"Shield not activated yet. Hits remaining: {maxHitsToActivateShield - currentHits}");
        }

        health -= damage;

        if (health <= 0)
        {
            Debug.Log("Enemy defeated!");
            Destroy(gameObject);
        }
    }

    private void ActivateShield()
    {
        if (shieldRenderer != null) shieldRenderer.enabled = true; // Enable shield visibility
        if (shieldCollider != null) shieldCollider.enabled = true; // Enable shield functionality

        isShieldActive = true;
        Debug.Log("Shield activated!");

        if (!shieldRenderer.enabled)
        {
            Debug.LogError("Shield renderer failed to activate! Check the object settings.");
        }
    }

    private void DeactivateShield()
    {
        if (shieldRenderer != null) shieldRenderer.enabled = false; // Disable shield visibility
        if (shieldCollider != null) shieldCollider.enabled = false; // Disable shield functionality

        isShieldActive = false;
        currentHits = 0; // Reset hits for reactivation
        Debug.Log("Shield deactivated!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
