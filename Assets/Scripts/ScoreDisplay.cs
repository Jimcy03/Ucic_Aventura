using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public string scorePrefix = "Enemies: ";
    
    void Update()
    {
        if (ScoreManager.Instance != null && scoreText != null)
        {
            scoreText.text = scorePrefix + ScoreManager.Instance.GetCurrentScore().ToString();
        }
    }
}