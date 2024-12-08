using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject ammoPrefab; // Assign your ammo prefab in the Inspector
    public float shootForce = 20f; // Adjust the force applied to the ammo
    public float shootOffset = 1.5f; // Distance in front of the player to spawn the ammo
    public float verticalOffset = 1.0f; // Height offset for bullet spawning

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Calculate the spawn position in front of the player on the horizontal axis with a vertical offset
        Vector3 shootPosition = transform.position 
                                + new Vector3(transform.forward.x, 0, transform.forward.z).normalized * shootOffset 
                                + new Vector3(0, verticalOffset, 0);

        // Instantiate ammo at the calculated position with the player's rotation
        GameObject ammoInstance = Instantiate(ammoPrefab, shootPosition, Quaternion.identity);

        // Apply force to the ammo's Rigidbody
        Rigidbody rb = ammoInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Ensure the ammo moves forward on the horizontal plane
            Vector3 shootDirection = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            rb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
        }
    }
}
