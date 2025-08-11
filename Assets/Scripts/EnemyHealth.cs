using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 30;
    public GameObject deathEffect;
    public int scoreValue = 1; // Puntos por eliminación

    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 1f, 0);
    public float healthBarDuration = 3f;

    private int currentHealth;
    private GameObject healthBarInstance;
    private Slider healthSlider;
    private float healthBarTimer;
    private bool healthBarVisible;

    void Start()
    {
        currentHealth = maxHealth;
        healthBarVisible = false;
    }

    void Update()
    {
        if (healthBarVisible && healthBarInstance != null)
        {
            healthBarInstance.transform.position = transform.position + healthBarOffset;
            healthBarTimer -= Time.deltaTime;
            if (healthBarTimer <= 0) HideHealthBar();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        ShowHealthBar();

        if (currentHealth <= 0) Die();
    }

    public void ShowHealthBar()
    {
        if (healthBarInstance == null && healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
            healthSlider = healthBarInstance.GetComponentInChildren<Slider>();
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }

            if (healthBarInstance.GetComponent<Billboard>() == null)
            {
                healthBarInstance.AddComponent<Billboard>();
            }
        }
        else if (healthBarInstance != null && healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

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

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        // AGREGADO: Destruye la barra de vida antes de destruir el enemigo.
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }

        // Destruye al enemigo.
        Destroy(gameObject);
    }
}
