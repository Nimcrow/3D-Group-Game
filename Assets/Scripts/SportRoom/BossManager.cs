using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // for multiple scenes
using TMPro; // For TextMeshPro

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;
    public int bossHealth;

    // Reference to the UI Text component
    public TextMeshProUGUI bossText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize the grade text on startup
        bossHealth = 200;
        UpdateBossText();
    }

    public void DamageBoss()
    {
        bossHealth--;
        UpdateBossText();
    }

    private void UpdateBossText()
    {
        if (bossText != null)
        {
            bossText.text = $"Boss Health: {bossHealth}";
        }
        else
        {
            Debug.LogWarning("Grade text is not assigned in the GameManager.");
        }
    }
}
