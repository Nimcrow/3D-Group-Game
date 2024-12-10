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
            Destroy(gameObject);
            return;
        }

        // Make the ammo extremely light so it imparts virtually no force
        rb.mass = 0.001f;

        // Disable gravity so it won't drop and lose momentum
        rb.useGravity = false;

        // Set collision detection to continuous to prevent passing through objects
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Destroy the ammo after its lifespan
        Destroy(gameObject, lifespan);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if we hit an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyAI enemyAI = collision.gameObject.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                // Apply damage to the enemy
                enemyAI.TakeDamage(damage);
            }
        }

        // Destroy the ammo upon collision
        Debug.Log($"Ammo hit: {collision.gameObject.name}");
        Destroy(gameObject);
    }
}
