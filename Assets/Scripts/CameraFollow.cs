using UnityEngine;

/// <summary>
/// Script para que la cámara siga a un objetivo (normalmente el jugador) en un juego 2D.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    // El Transform del objetivo que la cámara seguirá.
    // Asigna a tu jugador en el inspector.
    public Transform target;

    // Velocidad con la que la cámara se mueve hacia el objetivo.
    // Un valor más alto hace que el seguimiento sea más rápido.
    [Range(0.01f, 1.0f)]
    public float followSpeed = 0.5f;

    // Vector3 que representa el desplazamiento de la cámara con respecto al objetivo.
    // Esto te permite ajustar la posición de la cámara (por ejemplo, para que esté un poco por encima del jugador).
    public Vector3 offset;

    private void FixedUpdate()
    {
        // Verifica si el objetivo ha sido asignado.
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: El objetivo (target) no está asignado.");
            return;
        }

        // Calcula la posición deseada de la cámara.
        // La posición del objetivo + el desplazamiento.
        Vector3 desiredPosition = target.position + offset;

        // Suaviza la transición de la cámara desde su posición actual a la deseada.
        // Usa Lerp para un movimiento más fluido y menos brusco.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed);

        // Actualiza la posición de la cámara.
        transform.position = smoothedPosition;
    }
}
