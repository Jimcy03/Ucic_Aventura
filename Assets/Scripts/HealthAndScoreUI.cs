using TMPro;
using UnityEngine;

public class HealthAndScoreUI : MonoBehaviour
{
    [Header("Health Settings")]
    public TextMeshProUGUI healthText;
    public PlayerHealth playerHealth;
    
    [Header("Score Settings")]
    public TextMeshProUGUI scoreText;
    public string scorePrefix = "Score: ";
    
    void Update()
    {
        // Actualizar salud
        if (playerHealth != null && healthText != null)
        {
            healthText.text = playerHealth.CurrentHealth.ToString();
        }
        
        // Actualizar puntuación
        if (ScoreManager.Instance != null && scoreText != null)
        {
            scoreText.text = scorePrefix + ScoreManager.Instance.GetCurrentScore().ToString();
        }
    }
}