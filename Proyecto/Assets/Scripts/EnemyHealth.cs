using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 30;
    public GameObject deathEffect;
    
    private int currentHealth;
    private EnemySpawner spawner;

    void Start()
    {
        currentHealth = maxHealth;
        spawner = FindObjectOfType<EnemySpawner>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        if (spawner != null)
        {
            spawner.EnemyDestroyed();
        }
        
        Destroy(gameObject);
    }
}