using UnityEngine;

/// <summary>
/// Script que hace que un objeto siempre mire a la cámara.
/// </summary>
public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // La barra de vida siempre mirará a la cámara para ser legible.
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }

        // Si el objeto padre (enemigo) ha sido destruido, la barra de vida también debe destruirse.
        // Se destruye el GameObject al que está adjunto este script, que es el de la barra de vida.
        if (transform.parent == null)
        {
            Destroy(gameObject);
        }
    }
}
