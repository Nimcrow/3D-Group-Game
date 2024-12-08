using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject ammoPrefab; // Assign your ammo prefab in the Inspector
    public float shootForce = 20f; // Adjust the force applied to the ammo
    public float shootOffset = 1.5f; // Distance in front of the player to spawn the ammo
    public float verticalOffset = 1.0f; // Height offset for bullet spawning
    public Camera playerCamera; // Reference to the player's camera

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Raycast to detect where the cursor is pointing in the world
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit)) // If the ray hits something
        {
            targetPoint = hit.point;
        }
        else // Default to a point far away in the direction of the cursor
        {
            targetPoint = ray.GetPoint(100f); // 100 units away from the camera
        }

        // Calculate the direction from the player's position to the target point
        Vector3 shootDirection = (targetPoint - transform.position).normalized;

        // Adjust the spawn position for the ammo
        Vector3 shootPosition = transform.position + shootDirection * shootOffset + new Vector3(0, verticalOffset, 0);

        // Instantiate ammo at the calculated position
        GameObject ammoInstance = Instantiate(ammoPrefab, shootPosition, Quaternion.identity);

        // Apply force to the ammo's Rigidbody
        Rigidbody rb = ammoInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
        }
    }
}
