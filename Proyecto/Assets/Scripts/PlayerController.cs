using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 15f;
    public float deceleration = 20f;
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

        // Configurar física
        rb.linearDamping = 10f; // Mayor fricción
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

        // Sistema de movimiento con aceleración/deceleración
        Vector2 targetVelocity = moveInput * moveSpeed;
        Vector2 velocityChange = targetVelocity - rb.linearVelocity;

        // Aplicar fuerza con aceleración controlada
        if (moveInput.magnitude > 0.1f)
        {
            rb.AddForce(velocityChange * acceleration);
        }
        else
        {
            // Frenado más rápido cuando no hay input
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        // Limitar velocidad máxima
        if (rb.linearVelocity.magnitude > moveSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
    }

    void UpdateGuideLine()
    {
        // Obtener posición del mouse en el mundo
        Vector3 mousePosition = GetMouseWorldPosition();

        // Actualizar línea desde el jugador hasta el cursor
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, mousePosition);

        // Rotar el jugador hacia el cursor
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    // Detener completamente el movimiento al chocar con paredes
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paredes"))
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}