using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public float invincibilityTime = 1f;
    
    private int currentHealth;
    private bool isInvincible;
    private float invincibilityTimer;

    public int CurrentHealth => currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0) isInvincible = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        
        currentHealth -= damage;
        isInvincible = true;
        invincibilityTimer = invincibilityTime;
        
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        gameObject.SetActive(false);
    }
}