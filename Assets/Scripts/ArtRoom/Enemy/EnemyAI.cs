using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject finishPoint;
    private Transform player;
    private Animator animator;

    public LayerMask whatIsPlayer;

    [SerializeField] private float health = 100f;

    // Attacking
    [SerializeField] private float timeBetweenAttacks = 1f;
    private bool alreadyAttacked;
    private bool isAttacking;
    public GameObject projectile;
    public GameObject specialProjectile;
    [SerializeField] private float verticalOffset = 1f;
    private int attackCounter = 0;
    public Transform throwPoint;

    // States
    [SerializeField] private float sightRange = 10f;
    [SerializeField] private float attackRange = 5f;
    private bool playerInSightRange, playerInAttackRange;

    // Movement
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float enragedChaseSpeed = 8f;

    // Enraged State
    [SerializeField] private float enragedInterval = 10f;
    [SerializeField] private float enragedDuration = 4f;
    private bool isEnraged = false;

    // Shield visual for enraged state
    public GameObject shield;
    private MeshRenderer shieldRenderer;
    private SphereCollider shieldCollider;

    // Player collision damage
    [SerializeField] private int collisionDamage = 10;

    // Damage Cooldown for proximity damage while enraged
    private float nextDamageTime = 0f;
    private float damageInterval = 1f;

    // Audio
    [SerializeField] private AudioClip enrageSound; // Enrage sound effect
    [SerializeField] private AudioClip attackSound; // Attack sound effect
    [SerializeField] private AudioClip walkingSound; // Walking sound effect
    private AudioSource audioSource;

    private bool isWalkingSoundPlaying = false; // Track if walking sound is playing

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

        // Ensure an AudioSource is attached
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

        ManageWalkingSound();
    }

    private void HandleEnragedState()
    {
        if (isAttacking)
        {
            ResetAttack();
            animator.ResetTrigger("Attack");
        }

        animator.SetBool("isRunning", true);
        ChasePlayer(enragedChaseSpeed);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= 0.25f && Time.time >= nextDamageTime)
        {
            ApplyCollisionDamage();
            nextDamageTime = Time.time + damageInterval;
        }
    }

    private void HandleNormalState()
    {
        if (playerInAttackRange)
        {
            AttackPlayer();
        }
        else
        {
            if (isAttacking)
            {
                ResetAttack();
                animator.ResetTrigger("Attack");
            }

            if (!isAttacking && playerInSightRange)
            {
                ChasePlayer(chaseSpeed);
            }
        }

        bool shouldRun = !playerInAttackRange && !isAttacking && !isEnraged && playerInSightRange;
        animator.SetBool("isRunning", shouldRun);
    }

    private IEnumerator EnragedCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(enragedInterval);

            isEnraged = true;
            EnableShield(true);

            // Play enrage sound
            PlaySound(enrageSound);

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

        if (!alreadyAttacked)
        {
            isAttacking = true;
            attackCounter++;
            animator.SetTrigger("Attack");

            // Play attack sound
            PlaySound(attackSound);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
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
    }

    private void ApplyCollisionDamage()
    {
        PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(collisionDamage);
        }
    }

    private void ManageWalkingSound()
    {
        bool isWalking = animator.GetBool("isRunning") && !isAttacking && !isEnraged;

        if (isWalking && !isWalkingSoundPlaying)
        {
            PlaySoundLoop(walkingSound);
            isWalkingSoundPlaying = true;
        }
        else if (!isWalking && isWalkingSoundPlaying)
        {
            StopSound();
            isWalkingSoundPlaying = false;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void PlaySoundLoop(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void StopSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isEnraged)
        {
            return;
        }

        health -= damage;

        if (health <= 0)
        {
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
}
