using TMPro; // Import TextMeshPro namespace
using UnityEngine;

public class GradeDisplay : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth; // Reference to the PlayerHealth script
    public TextMeshProUGUI gradeText; // Reference to the TextMeshPro UI element

    private void Start()
    {
        // Auto-find the PlayerHealth script if not assigned
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
            Debug.Log($"PlayerHealth found: {playerHealth != null}");
        }

        // Ensure gradeText is assigned
        if (gradeText == null)
        {
            Debug.LogError("Grade Text UI element is not assigned!");
        }
        else
        {
            Debug.Log("Grade Text UI element is assigned.");
        }

        // Initialize the UI with the current grade
        RefreshGrade();
    }

    public void RefreshGrade()
    {
        // Update the grade text UI element
        if (playerHealth != null && gradeText != null)
        {
            gradeText.text = $"Grade: {playerHealth.GetCurrentGrade()}";
            Debug.Log($"Grade updated to: {playerHealth.GetCurrentGrade()}");
        }
        else
        {
            Debug.LogError("PlayerHealth or GradeText is missing!");
        }
    }
}
