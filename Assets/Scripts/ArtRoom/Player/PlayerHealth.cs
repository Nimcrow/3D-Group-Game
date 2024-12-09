using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // Maximum health
    private int currentHealth;
    private bool isInvincible = false; // Flag for invincibility

    [Header("Respawn Settings")]
    public RespawnSystem respawnSystem; // Reference to the RespawnSystem

    private void Start()
    {
        currentHealth = maxHealth;

        // Auto-find RespawnSystem if not assigned
        if (respawnSystem == null)
        {
            respawnSystem = FindObjectOfType<RespawnSystem>();
        }

        if (respawnSystem == null)
        {
            Debug.LogError("No RespawnSystem found! Assign one in the Inspector.");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            Debug.Log("Player is invincible! Damage ignored.");
            return; // Ignore damage during invincibility
        }

        if (damage <= 0) return; // Ignore non-positive damage values

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");

        // Respawn player and reset health
        if (respawnSystem != null)
        {
            respawnSystem.RespawnPlayer(gameObject);
            ResetHealth();
        }
        else
        {
            Debug.LogError("RespawnSystem is missing! Player cannot respawn.");
        }
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        Debug.Log("Player health reset.");
    }

    public void Heal(int amount)
    {
        if (amount > 0)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            Debug.Log($"Player healed by {amount}. Current health: {currentHealth}");
        }
    }

    // Invincibility logic
    public void StartInvincibility(float duration)
    {
        if (isInvincible) return; // Don't restart invincibility if already active
        StartCoroutine(InvincibilityCoroutine(duration));
    }

    private System.Collections.IEnumerator InvincibilityCoroutine(float duration)
    {
        isInvincible = true;
        Debug.Log("Player is now invincible!");

        // Optional: Add visual feedback for invincibility (e.g., flashing effect)
        yield return new WaitForSeconds(duration);

        isInvincible = false;
        Debug.Log("Player is no longer invincible!");
    }
}
