using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    public Transform respawnPoint; // Fixed respawn point
    public float invincibilityDuration = 2f; // Duration of invincibility after respawn

    public void RespawnPlayer(GameObject player)
    {
        if (respawnPoint != null && player != null)
        {
            Debug.Log($"Respawning player at: {respawnPoint.position}");
            Debug.Log($"Current player position before respawn: {player.transform.position}");

            // Disable player temporarily to prevent interference
            player.SetActive(false);

            // Set position and rotation
            player.transform.position = respawnPoint.position;
            player.transform.rotation = respawnPoint.rotation;

            // Reset Rigidbody velocity
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Re-enable player
            player.SetActive(true);

            // Grant invincibility frames
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.StartInvincibility(invincibilityDuration);
            }

            Debug.Log($"Player respawned at: {player.transform.position}");
        }
        else
        {
            Debug.LogError("Respawn point or player is missing!");
        }
    }
}
