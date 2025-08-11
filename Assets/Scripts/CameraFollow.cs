using UnityEngine;

/// <summary>
/// Script para que la c�mara siga a un objetivo (normalmente el jugador) en un juego 2D.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    // El Transform del objetivo que la c�mara seguir�.
    // Asigna a tu jugador en el inspector.
    public Transform target;

    // Velocidad con la que la c�mara se mueve hacia el objetivo.
    // Un valor m�s alto hace que el seguimiento sea m�s r�pido.
    [Range(0.01f, 1.0f)]
    public float followSpeed = 0.5f;

    // Vector3 que representa el desplazamiento de la c�mara con respecto al objetivo.
    // Esto te permite ajustar la posici�n de la c�mara (por ejemplo, para que est� un poco por encima del jugador).
    public Vector3 offset;

    private void FixedUpdate()
    {
        // Verifica si el objetivo ha sido asignado.
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: El objetivo (target) no est� asignado.");
            return;
        }

        // Calcula la posici�n deseada de la c�mara.
        // La posici�n del objetivo + el desplazamiento.
        Vector3 desiredPosition = target.position + offset;

        // Suaviza la transici�n de la c�mara desde su posici�n actual a la deseada.
        // Usa Lerp para un movimiento m�s fluido y menos brusco.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed);

        // Actualiza la posici�n de la c�mara.
        transform.position = smoothedPosition;
    }
}
