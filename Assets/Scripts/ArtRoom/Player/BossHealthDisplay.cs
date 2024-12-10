using TMPro;
using UnityEngine;

public class BossHealthDisplay : MonoBehaviour
{
    [Header("References")]
    public EnemyAI enemyAI; // Reference to the EnemyAI script
    public TextMeshProUGUI bossHealthText; // Reference to the TextMeshPro UI element

    private void Start()
    {
        if (enemyAI == null)
        {
            enemyAI = FindObjectOfType<EnemyAI>();
            Debug.Log($"EnemyAI found: {enemyAI != null}");
        }

        if (bossHealthText == null)
        {
            Debug.LogError("Boss Health Text UI element is not assigned!");
            return;
        }

        if (enemyAI != null)
        {
            UpdateHealthUI();
        }
    }

    private void Update()
    {
        if (enemyAI != null)
        {
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        if (bossHealthText != null && enemyAI != null)
        {
            bossHealthText.text = $"Boss Health: {Mathf.CeilToInt(enemyAI.Health)}";
        }
    }
}
