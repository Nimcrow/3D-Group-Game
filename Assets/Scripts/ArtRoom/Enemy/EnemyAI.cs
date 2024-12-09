using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject playerObject; // Drag the player GameObject here in the Inspector
    public GameObject finishPoint; // Reference to the FinishPoint GameObject
    private Transform player;       // Reference to the player's Transform
    private Animator animator;      // Reference to the Animator component

    public LayerMask whatIsPlayer;

    public float health = 100;

    // Attacking
    public float timeBetweenAttacks = 1f;
    private bool alreadyAttacked;
    public GameObject projectile; // Regular projectile
    public GameObject specialProjectile; // Special projectile
    public float verticalOffset = 1f; // Vertical offset for projectile spawn position
    private int attackCounter = 0; // Counter to track attacks

    // States
    public float sightRange = 10f, attackRange = 5f;
    private bool playerInSightRange, playerInAttackRange;

    // Movement
    public float chaseSpeed = 4f; // Speed for chasing

    // Shield
    public GameObject shield; // Reference to the shield GameObject
    public int maxHitsToActivateShield = 4;
    private int currentHits = 0;
    private bool isShieldActive = false;
    private MeshRenderer shieldRenderer;
    private SphereCollider shieldCollider;

    // Player collision damage
    [SerializeField] private int collisionDamage = 10; // Damage dealt to the player on collision

    private void Awake()
    {
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        animator = GetComponent<Animator>(); // Get the Animator component

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

        if (playerInSightRange)
        {
            ChasePlayer(); // Always chase when player is detected
        }

        if (playerInAttackRange)
        {
            AttackPlayer(); // Attack when in attack range
        }

        // Update running animation state (run while chasing or attacking)
        if (animator != null)
        {
            animator.SetBool("isRunning", playerInSightRange);
        }
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

            if (chosenProjectile != null)
            {
                Vector3 spawnPosition = transform.position + transform.forward + Vector3.up * verticalOffset;
                GameObject projectileInstance = Instantiate(chosenProjectile, spawnPosition, Quaternion.identity);

                Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                }

                // Assign "splat trigger" logic if special projectile
                if (attackCounter % 4 == 0)
                {
                    SpecialProjectile special = projectileInstance.GetComponent<SpecialProjectile>();
                    if (special != null)
                    {
                        special.AssignPlayer(playerObject);
                    }
                }
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
        if (currentHits >= maxHitsToActivateShield)
        {
            ActivateShield();
        }

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

    private void ActivateShield()
    {
        if (shieldRenderer != null) shieldRenderer.enabled = true;
        if (shieldCollider != null) shieldCollider.enabled = true;

        isShieldActive = true;
    }

    private void DeactivateShield()
    {
        if (shieldRenderer != null) shieldRenderer.enabled = false;
        if (shieldCollider != null) shieldCollider.enabled = false;

        isShieldActive = false;
        currentHits = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == playerObject)
        {
            // Assuming the player has a script with a TakeDamage method
            var playerHealth = playerObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(collisionDamage);
            }
        }
    }
}
