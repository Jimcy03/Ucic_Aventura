using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 30;
    public GameObject deathEffect;
    public int scoreValue = 10;
    
    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 1f, 0);
    public float healthBarDuration = 3f;
    
    [Header("References")]
    public EnemySpawner spawner;

    private int currentHealth;
    private GameObject healthBarInstance;
    private Slider healthSlider;
    private float healthBarTimer;
    private bool healthBarVisible;

    public void SetSpawner(EnemySpawner spawnerReference)
    {
        spawner = spawnerReference;
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBarVisible = false;
    }

    void Update()
    {
        // Actualizar posición de la barra de salud
        if (healthBarVisible && healthBarInstance != null)
        {
            healthBarInstance.transform.position = transform.position + healthBarOffset;
            
            // Temporizador para ocultar la barra
            healthBarTimer -= Time.deltaTime;
            if (healthBarTimer <= 0)
            {
                HideHealthBar();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Mostrar/actualizar barra de salud
        ShowHealthBar();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ShowHealthBar()
    {
        if (healthBarInstance == null && healthBarPrefab != null)
        {
            // Crear la barra de salud si no existe
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
            healthSlider = healthBarInstance.GetComponentInChildren<Slider>();
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
            
            // Hacer que la barra mire siempre a la cámara
            if (healthBarInstance.GetComponent<Billboard>() == null)
            {
                healthBarInstance.AddComponent<Billboard>();
            }
        }
        else if (healthBarInstance != null && healthSlider != null)
        {
            // Actualizar valor si ya existe
            healthSlider.value = currentHealth;
        }
        
        // Mostrar y reiniciar temporizador
        if (healthBarInstance != null)
        {
            healthBarInstance.SetActive(true);
            healthBarTimer = healthBarDuration;
            healthBarVisible = true;
        }
    }

    public void HideHealthBar()
    {
        if (healthBarInstance != null)
        {
            healthBarInstance.SetActive(false);
            healthBarVisible = false;
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
        
        // Sistema de puntuación
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }
        else
        {
            Debug.LogWarning("ScoreManager instance not found!");
        }
        
        // Destruir barra de salud si existe
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
        
        Destroy(gameObject);
    }
}