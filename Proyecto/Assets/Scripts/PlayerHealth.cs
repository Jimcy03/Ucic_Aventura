using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public float invincibilityTime = 1f;
    
    private int currentHealth;
    private bool isInvincible;
    private float invincibilityTimer;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        
        currentHealth -= damage;
        isInvincible = true;
        invincibilityTimer = invincibilityTime;
        
        Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Aquí implementar lógica de muerte (respawn, game over, etc.)
    }
}