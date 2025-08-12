using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public PlayerHealth playerHealth;
    
    void Update()
    {
        if (playerHealth != null && healthText != null)
        {
            healthText.text = playerHealth.CurrentHealth.ToString();
        }
    }
}