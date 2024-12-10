using UnityEngine;

public class SpecialProjectile : MonoBehaviour
{
    private GameObject player; // Reference to the player object

    [Header("Projectile Settings")]
    [SerializeField] private float lifeTime = 5f;       // Time before the projectile is destroyed
    [SerializeField] private float initialSpeed = 10f;  // Initial speed of the projectile
    [SerializeField] private float slowDownRate = 0f;   // Rate at which the projectile slows down per second
    [SerializeField] private int damage = 10;           // Damage the projectile deals to the player

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Set Rigidbody settings for fast-moving objects
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            // Set the initial velocity of the projectile
            rb.velocity = transform.forward * initialSpeed;
        }

        // Schedule the projectile to be destroyed after its lifetime expires
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (rb != null && slowDownRate > 0f)
        {
            // Reduce the projectile's velocity over time
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, slowDownRate * Time.fixedDeltaTime);
        }
    }

    public void AssignPlayer(GameObject targetPlayer)
    {
        player = targetPlayer; // Assign the player when the projectile is instantiated
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Projectile collided with {collision.gameObject.name}");

        // Check if the projectile hit something tagged as "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by special projectile!");

            // Attempt to find the player's health component
            PlayerHealth playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Special projectile hit player, dealing {damage} damage.");
            }
            else
            {
                Debug.LogWarning("No PlayerHealth component found on the player or its parents.");
            }

            // Trigger the splat effect
            SplatEffect splat = collision.gameObject.GetComponentInParent<SplatEffect>();
            if (splat != null)
            {
                splat.ShowSplat();
            }
            else
            {
                Debug.LogWarning("No SplatEffect component found on the player or its parents.");
            }
        }

        // Destroy the projectile after any collision
        Destroy(gameObject);
    }
}
