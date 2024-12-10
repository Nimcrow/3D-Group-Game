using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // Maximum health
    private int currentHealth;
    private bool isInvincible = false; // Flag for invincibility

    [Header("Respawn Settings")]
    public RespawnSystem respawnSystem; // Reference to the RespawnSystem

    [Header("Score System")]
    private string[] grades = { "A", "B", "C", "D", "F" }; // Grade scale
    private int gradeIndex = 0; // Index for current grade

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

        Debug.Log($"PlayerHealth initialized. Starting grade: {grades[gradeIndex]}");
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

        // Update the player's grade
        UpdateGrade();

        // Notify the GradeDisplay to refresh the UI
        GradeDisplay gradeDisplay = FindObjectOfType<GradeDisplay>();
        if (gradeDisplay != null)
        {
            gradeDisplay.RefreshGrade();
        }

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

    // Update the player's grade
    private void UpdateGrade()
    {
        if (gradeIndex < grades.Length - 1)
        {
            gradeIndex++;
            Debug.Log($"Grade decreased! New grade: {grades[gradeIndex]}");
        }
        else
        {
            Debug.Log("Player has already reached the lowest grade (F).");
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

    public string GetCurrentGrade()
    {
        return grades[gradeIndex];
    }
}
