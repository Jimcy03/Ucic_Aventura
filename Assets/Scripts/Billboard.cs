using UnityEngine;

/// <summary>
/// Script que hace que un objeto siempre mire a la c�mara.
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
            // La barra de vida siempre mirar� a la c�mara para ser legible.
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }

        // Si el objeto padre (enemigo) ha sido destruido, la barra de vida tambi�n debe destruirse.
        // Se destruye el GameObject al que est� adjunto este script, que es el de la barra de vida.
        if (transform.parent == null)
        {
            Destroy(gameObject);
        }
    }
}
