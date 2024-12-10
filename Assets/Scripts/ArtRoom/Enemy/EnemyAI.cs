using UnityEngine;
using System.Collections; // Needed for Coroutines

public class EnemyAI : MonoBehaviour
{
    public GameObject playerObject; // Drag the player GameObject here in the Inspector
    public GameObject finishPoint; // Reference to the FinishPoint GameObject
    private Transform player;       // Reference to the player's Transform
    private Animator animator;      // Reference to the Animator component

    public LayerMask whatIsPlayer;

    [SerializeField] private float health = 100f;

    // Attacking
    [SerializeField] private float timeBetweenAttacks = 1f;
    private bool alreadyAttacked;
    private bool isAttacking; // Indicates the enemy is currently in attack mode (no movement)
    public GameObject projectile; // Regular projectile
    public GameObject specialProjectile; // Special projectile
    [SerializeField] private float verticalOffset = 1f; // Vertical offset if needed
    private int attackCounter = 0; // Counter to track attacks
    public Transform throwPoint; // Assign in Inspector

    // States
    [SerializeField] private float sightRange = 10f;
    [SerializeField] private float attackRange = 5f;
    private bool playerInSightRange, playerInAttackRange;

    // Movement
    [SerializeField] private float chaseSpeed = 4f; // Normal speed for chasing
    [SerializeField] private float enragedChaseSpeed = 8f; // Enraged state chase speed

    // Enraged State
    [SerializeField] private float enragedInterval = 10f;  // Time between becoming enraged
    [SerializeField] private float enragedDuration = 4f;   // Duration of enraged state
    private bool isEnraged = false;                        // Whether the enemy is currently enraged

    // Shield visual for enraged state
    public GameObject shield;
    private MeshRenderer shieldRenderer;
    private SphereCollider shieldCollider;

    // Player collision damage
    [SerializeField] private int collisionDamage = 10; // Damage dealt to the player on collision
    
    // Damage Cooldown for proximity damage while enraged
    private float nextDamageTime = 0f;
    private float damageInterval = 1f; // Apply damage once every second while close

    private void Awake()
    {
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        animator = GetComponent<Animator>(); // Get the Animator component

        // Prepare the shield for enraged visuals
        if (shield != null)
        {
            shieldRenderer = shield.GetComponent<MeshRenderer>();
            shieldCollider = shield.GetComponent<SphereCollider>();

            if (shieldRenderer != null) shieldRenderer.enabled = false;
            if (shieldCollider != null) shieldCollider.enabled = false;
        }
    }

    private void Start()
    {
        // Start the enraged cycle
        StartCoroutine(EnragedCycle());
    }

    private void Update()
    {
        if (player == null) return;

        // Always face the player
        transform.LookAt(player);

        // Check for sight and attack range only if not enraged
        if (!isEnraged)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        }

        if (isEnraged)
        {
            // If currently attacking and we become or remain enraged, stop attacking
            if (isAttacking)
            {
                ResetAttack();
                animator.ResetTrigger("Attack");
            }

            // Enraged: No attacking, just run straight to the player with shield on.
            animator.SetBool("isRunning", true);
            ChasePlayer(enragedChaseSpeed);

            // Check player distance for proximity damage
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= 0.25f && Time.time >= nextDamageTime)
            {
                // Player close enough to take damage
                ApplyCollisionDamage();
                nextDamageTime = Time.time + damageInterval;
            }
        }
        else
        {
            // Normal behavior
            if (playerInAttackRange)
            {
                AttackPlayer();
            }
            else
            {
                // If the player moved out of attack range while attacking, interrupt the attack
                if (isAttacking)
                {
                    ResetAttack();
                    animator.ResetTrigger("Attack");
                }

                // If not attacking and player is in sight, chase
                if (!isAttacking && playerInSightRange)
                {
                    ChasePlayer(chaseSpeed);
                }
            }

            // Update running animation state:
            // Run if not enraged, not in attack range, and not attacking.
            bool shouldRun = !playerInAttackRange && !isAttacking && !isEnraged && playerInSightRange;
            animator.SetBool("isRunning", shouldRun);
        }
    }

    private IEnumerator EnragedCycle()
    {
        while (true)
        {
            // Wait for the interval before becoming enraged
            yield return new WaitForSeconds(enragedInterval);

            // Enter enraged state
            isEnraged = true;
            EnableShield(true);

            // During enragedDuration, the enemy is untargetable and just chases the player
            yield return new WaitForSeconds(enragedDuration);

            // Exit enraged state
            isEnraged = false;
            EnableShield(false);
        }
    }

    private void EnableShield(bool enable)
    {
        if (shieldRenderer != null) shieldRenderer.enabled = enable;
        if (shieldCollider != null) shieldCollider.enabled = enable;
    }

    private void ChasePlayer(float speed)
    {
        // Move directly towards the player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void AttackPlayer()
    {
        // Don't attack if enraged
        if (isEnraged) return;

        // Double-check the player is still in range before attacking
        if (!playerInAttackRange)
            return;

        // If not already attacking, start an attack
        if (!alreadyAttacked)
        {
            isAttacking = true; // Enemy is now attacking; should not move
            attackCounter++;
            // Trigger the animation that includes the throwing event
            animator.SetTrigger("Attack");

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    // Called from the animation event
    public void SpawnProjectile()
    {
        if (throwPoint == null)
        {
            Debug.LogWarning("ThrowPoint is not assigned on " + gameObject.name);
            return;
        }

        // Determine which projectile to shoot
        GameObject chosenProjectile = (attackCounter % 4 == 0) ? specialProjectile : projectile;
        if (chosenProjectile == null)
        {
            Debug.LogWarning("No projectile assigned!");
            return;
        }

        // Create a horizontal direction vector (no upward component)
        Vector3 horizontalDirection = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(horizontalDirection);
        Vector3 spawnPosition = throwPoint.position + (Vector3.up * verticalOffset);

        GameObject projectileInstance = Instantiate(chosenProjectile, spawnPosition, spawnRotation);

        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(horizontalDirection * 32f, ForceMode.Impulse);
        }

        // If it's the special projectile, assign the player
        if (attackCounter % 4 == 0)
        {
            SpecialProjectile special = projectileInstance.GetComponent<SpecialProjectile>();
            if (special != null)
            {
                special.AssignPlayer(playerObject);
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        isAttacking = false; // Attacking is done, can move/run again if needed
    }

    private void ApplyCollisionDamage()
    {
        PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(collisionDamage);
            Debug.Log($"Player took {collisionDamage} damage due to close proximity in enraged state.");
        }
    }

    public void TakeDamage(int damage)
    {
        // If enraged, the enemy is untargetable and takes no damage
        if (isEnraged)
        {
            Debug.Log("Attack ignored because the enemy is enraged and untargetable!");
            return;
        }

        // Take damage normally
        health -= damage;

        if (health <= 0)
        {
            Debug.Log("Enemy defeated!");

            // Activate the finish point when the enemy is defeated
            if (finishPoint != null)
            {
                FinishPoint finishPointScript = finishPoint.GetComponent<FinishPoint>();
                if (finishPointScript != null)
                {
                    finishPointScript.ActivateFinishPoint();
                }
            }

            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Normal collision logic applies outside enraged proximity damage
        if (collision.gameObject == playerObject)
        {
            var playerHealth = playerObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(collisionDamage);
            }
        }
    }
}
