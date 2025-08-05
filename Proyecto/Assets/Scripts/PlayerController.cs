using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Color lineColor = Color.yellow;
    public float lineWidth = 0.1f;
    public string sortingLayerName = "Player";
    public int sortingOrder = 1;

    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private Rigidbody2D rb;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        // Configurar LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;

        // Configurar orden de renderizado
        lineRenderer.sortingLayerName = sortingLayerName;
        lineRenderer.sortingOrder = sortingOrder;

        // Congelar rotación en el Rigidbody
        rb.freezeRotation = true;
    }

    void Update()
    {
        UpdateGuideLine();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector2 moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        // Mover usando física para colisiones
        rb.linearVelocity = moveInput * moveSpeed;
    }

    void UpdateGuideLine()
    {
        // Obtener posición del mouse en el mundo
        Vector3 mousePosition = GetMouseWorldPosition();

        // Actualizar línea desde el jugador hasta el cursor
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, mousePosition);

    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
}