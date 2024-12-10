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
        // Check if the projectile hit the player
        if (collision.gameObject == player)
        {
            Debug.Log("Player hit by special projectile!");

            // Deal damage to the player if possible
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Special projectile hit {player.name}, dealing {damage} damage.");
            }

            // Trigger the splat effect
            SplatEffect splat = player.GetComponent<SplatEffect>();
            if (splat != null)
            {
                splat.ShowSplat();
            }
        }

        // Destroy the projectile after any collision
        Destroy(gameObject);
    }
}
