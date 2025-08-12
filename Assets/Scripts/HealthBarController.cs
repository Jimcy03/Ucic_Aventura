using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Slider healthSlider;
    public Image fillImage;
    public Gradient healthGradient;

    void Start()
    {
        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
        }
        if (fillImage == null && healthSlider != null)
        {
            fillImage = healthSlider.fillRect.GetComponent<Image>();
        }
        
        // Inicializar gradiente si se ha asignado
        if (fillImage != null && healthGradient != null)
        {
            fillImage.color = healthGradient.Evaluate(1f);
        }
    }

    public void SetHealth(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            
            // Actualizar color según la salud
            if (fillImage != null && healthGradient != null)
            {
                float healthPercent = currentHealth / maxHealth;
                fillImage.color = healthGradient.Evaluate(healthPercent);
            }
        }
    }
}