using UnityEngine;

public class SpecialProjectile : MonoBehaviour
{
    private GameObject player; // Reference to the player object

    [Header("Projectile Settings")]
    [SerializeField] private float lifeTime = 5f; // Time before the projectile is destroyed

    private void Start()
    {
        // Schedule the projectile to be destroyed after its lifetime expires
        Destroy(gameObject, lifeTime);
    }

    public void AssignPlayer(GameObject targetPlayer)
    {
        player = targetPlayer; // Assign the player when the projectile is instantiated
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            Debug.Log("Player hit by special projectile!");

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
