using UnityEngine;

public class Ammo : MonoBehaviour
{
    public float lifespan = 5f; // Time before the ammo is destroyed
    public int damage = 10; // Damage dealt by the ammo

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on ammo prefab!");
            Destroy(gameObject); // Destroy ammo if Rigidbody is missing
            return;
        }

        Destroy(gameObject, lifespan); // Destroy the ammo after its lifespan
    }

    void Update()
    {
        // Debug check: Log velocity to confirm ammo is moving
        if (rb != null)
        {
            if (rb.velocity.magnitude > 0.1f)
            {
                Debug.Log($"Ammo is flying! Velocity: {rb.velocity}");
            }
            else
            {
                Debug.Log("Ammo is not moving!");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Handle collision with the enemy
        if (collision.gameObject.CompareTag("Enemy")) // Ensure the enemy has the "Enemy" tag
        {
            EnemyAI enemyAI = collision.gameObject.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(damage); // Trigger the enemy's damage logic
            }
        }

        // Destroy the ammo upon collision
        Debug.Log($"Ammo hit: {collision.gameObject.name}");
        Destroy(gameObject);
    }
}
