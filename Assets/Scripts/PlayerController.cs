using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Color lineColor = Color.yellow;
    public float lineWidth = 0.1f;
    public string sortingLayerName = "Player";
    public int sortingOrder = 1;

    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;

        lineRenderer.sortingLayerName = sortingLayerName;
        lineRenderer.sortingOrder = sortingOrder;

        rb.freezeRotation = true;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        UpdateGuideLine();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    void UpdateGuideLine()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, mousePosition);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        mouseScreenPosition.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
}
