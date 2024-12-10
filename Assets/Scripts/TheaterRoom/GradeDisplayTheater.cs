using TMPro;
using UnityEngine;

public class GradeDisplayTheater : MonoBehaviour
{
    [Header("References")]
    public MapDanceMove executeDance; // Reference to the PlayerHealth script
    public TextMeshProUGUI gradeText; // Reference to the TextMeshPro UI element

    private void Start()
    {
        // Initialize the UI with the current grade
        RefreshGrade();
    }

    public void RefreshGrade()
    {
        if (executeDance != null && gradeText != null)
        {
            gradeText.text = $"Grade: {executeDance.GetCurrentGrade()}";
            Debug.Log($"Grade updated to: {executeDance.GetCurrentGrade()}");
        }
        else
        {
            Debug.LogError("PlayerHealth or GradeText is missing!");
        }
    }
}
