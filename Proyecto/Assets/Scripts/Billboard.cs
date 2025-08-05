using UnityEngine;

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
            // Hacer que el objeto siempre mire a la cámara
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}