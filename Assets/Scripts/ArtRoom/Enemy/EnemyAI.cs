using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public GameObject playerObject;
    public GameObject finishPoint;
    public LayerMask whatIsPlayer;

    private Transform player;
    private Animator animator;

    [Header("Enemy Settings")]
    [SerializeField] private float health = 100f;
    public float Health => health;

    [Header("Attack Settings")]
    [SerializeField] private float timeBetweenAttacks = 1f;
    private bool alreadyAttacked;
    private bool isAttacking;
    public GameObject projectile;         
    public GameObject specialProjectile;  
    [SerializeField] private float verticalOffset = 1f;
    private int attackCounter = 0;
    public Transform throwPoint;

    [Header("Ranges")]
    [SerializeField] private float sightRange = 10f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float proximityRange = 1.0f;
    private bool playerInSightRange, playerInAttackRange;

    [Header("Speeds")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float enragedChaseSpeed = 8f;

    [Header("Enraged State")]
    [SerializeField] private float enragedInterval = 10f;
    [SerializeField] private float enragedDuration = 4f;
    private bool isEnraged = false;

    [Header("Shield")]
    public GameObject shield;
    private MeshRenderer shieldRenderer;
    private SphereCollider shieldCollider;

    [Header("Collision Damage")]
    [SerializeField] private int collisionDamage = 10;
    private float nextDamageTime = 0f;
    private float damageInterval = 1f;

    [Header("Audio")]
    [SerializeField] private AudioClip enragedSound;
    [SerializeField] private AudioClip runningSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;

    private void Awake()
    {
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        animator = GetComponent<Animator>();

        if (shield != null)
        {
            shieldRenderer = shield.GetComponent<MeshRenderer>();
            shieldCollider = shield.GetComponent<SphereCollider>();

            if (shieldRenderer != null) shieldRenderer.enabled = false;
            if (shieldCollider != null) shieldCollider.enabled = false;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        StartCoroutine(EnragedCycle());
    }

    private void Update()
    {
        if (player == null) return;

        transform.LookAt(player);

        // Only check ranges if not enraged
        if (!isEnraged)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        }

        if (isEnraged)
        {
            HandleEnragedState();
        }
        else
        {
            HandleNormalState();
        }
    }

    private void HandleEnragedState()
    {
        if (isAttacking)
        {
            // Don't reset attack prematurely; just wait for ResetAttack() to run
            animator.ResetTrigger("Attack");
        }

        animator.SetBool("isRunning", true);

        if (runningSound != null && !audioSource.isPlaying)
        {
            PlayRunningSound();
        }

        ChasePlayer(enragedChaseSpeed);

        if (Physics.CheckSphere(transform.position, proximityRange, whatIsPlayer) && Time.time >= nextDamageTime)
        {
            ApplyCollisionDamage();
            nextDamageTime = Time.time + damageInterval;
        }
    }

    private void HandleNormalState()
    {
        // If currently attacking, do not move or do anything else until attack resets
        if (isAttacking)
        {
            animator.SetBool("isRunning", false);
            return;
        }

        // If we are not currently attacking:
        if (playerInAttackRange)
        {
            // Attack if we haven't already attacked recently
            if (!alreadyAttacked)
            {
                AttackPlayer();
            }
            else
            {
                // We attacked recently and are waiting for ResetAttack
                // Stand still and wait
                animator.SetBool("isRunning", false);
            }
        }
        else
        {
            // Player not in attack range
            if (playerInSightRange)
            {
                // Chase the player
                animator.SetBool("isRunning", true);

                if (runningSound != null && !audioSource.isPlaying)
                {
                    PlayRunningSound();
                }

                ChasePlayer(chaseSpeed);
            }
            else
            {
                // Player not in sight range, stand still
                animator.SetBool("isRunning", false);
            }
        }
    }

    private IEnumerator EnragedCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(enragedInterval);

            isEnraged = true;
            EnableShield(true);
            PlayEnragedSound();

            yield return new WaitForSeconds(enragedDuration);

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
        Vector3 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void AttackPlayer()
    {
        if (isEnraged) return;
        if (!playerInAttackRange) return;

        isAttacking = true;
        attackCounter++;
        animator.SetTrigger("Attack");

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    public void SpawnProjectile()
    {
        if (throwPoint == null)
        {
            Debug.LogWarning("ThrowPoint is not assigned on " + gameObject.name);
            return;
        }

        GameObject chosenProjectile = (attackCounter % 4 == 0) ? specialProjectile : projectile;
        if (chosenProjectile == null)
        {
            Debug.LogWarning("No projectile assigned!");
            return;
        }

        Vector3 horizontalDirection = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(horizontalDirection);
        Vector3 spawnPosition = throwPoint.position + (Vector3.up * verticalOffset);

        GameObject projectileInstance = Instantiate(chosenProjectile, spawnPosition, spawnRotation);

        SpecialProjectile specialComp = projectileInstance.GetComponent<SpecialProjectile>();
        if (specialComp != null && playerObject != null)
        {
            specialComp.AssignPlayer(playerObject);
        }

        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(horizontalDirection * 32f, ForceMode.Impulse);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        isAttacking = false;
        animator.ResetTrigger("Attack");
    }

    private void ApplyCollisionDamage()
    {
        PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(collisionDamage);
            Debug.Log($"Player took {collisionDamage} damage due to proximity during enraged state.");

            // Play the impact sound if assigned
            if (audioSource != null && impactSound != null)
            {
                audioSource.PlayOneShot(impactSound);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isEnraged) return;

        health -= damage;
        if (health <= 0)
        {
            if (finishPoint != null)
            {
                FinishPoint finishPointScript = finishPoint.GetComponent<FinishPoint>();
                if (finishPointScript != null)
                {
                    // Play death sound before activating the finish point
                    PlayDeathSound();
                    finishPointScript.ActivateFinishPoint();
                }
            }
            else
            {
                // If there's no finish point, still play the death sound
                PlayDeathSound();
            }

            Destroy(gameObject);
        }
    }

    public void PlayEnragedSound()
    {
        if (audioSource != null && enragedSound != null)
        {
            audioSource.PlayOneShot(enragedSound);
        }
    }

    public void PlayRunningSound()
    {
        if (audioSource != null && runningSound != null)
        {
            audioSource.PlayOneShot(runningSound);
        }
    }

    public void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void PlayDeathSound()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
    }
}
