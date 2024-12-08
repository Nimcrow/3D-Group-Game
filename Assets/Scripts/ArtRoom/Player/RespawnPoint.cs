using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    public Transform respawnPoint; // Fixed respawn point

    public void RespawnPlayer(GameObject player)
    {
        if (respawnPoint != null && player != null)
        {
            player.transform.position = respawnPoint.position;
            player.transform.rotation = respawnPoint.rotation;
            Debug.Log("Player respawned at the respawn point.");
        }
        else
        {
            Debug.LogError("Respawn point or player is missing!");
        }
    }
}
