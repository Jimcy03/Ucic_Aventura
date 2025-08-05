using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("Cone Settings")]
    public float coneAngle = 30f;        // Ángulo del cono en grados
    public float fixedDistance = 15f;    // Distancia fija del cono
    public float damagePerSecond = 30f;  // Daño por segundo para enemigos dentro del cono

    [Header("Visual Settings")]
    public Color coneColor = new Color(1f, 0.5f, 0f, 0.3f); // Color naranja con transparencia
    public float coneWidth = 0.05f;
    public float coneDuration = 0.2f;
    public Transform gunEndPoint;
    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;
    public bool showConeRays = true;

    [Header("Light Effects")]
    public Light coneLight;              // Luz para el efecto de cono
    public float lightIntensity = 2f;
    public float lightRange = 15f;

    private Camera mainCamera;
    private Vector3 shotDirection;
    private bool isShooting = false;
    private LineRenderer coneRenderer;
    private bool coneActive = false;
    private float coneTimer = 0f;
    private Vector3 coneEndPoint;

    void Start()
    {
        mainCamera = Camera.main;
        CreateConeRenderer();
        ConfigureLight();
    }

    void ConfigureLight()
    {
        if (coneLight != null)
        {
            coneLight.type = LightType.Spot;
            coneLight.spotAngle = coneAngle;
            coneLight.range = lightRange;
            coneLight.intensity = lightIntensity;
            coneLight.color = coneColor;
            coneLight.enabled = false;
        }
    }

    void CreateConeRenderer()
    {
        // Crear un nuevo objeto para el cono
        GameObject coneObj = new GameObject("ConeRenderer");
        coneObj.transform.SetParent(transform);
        coneRenderer = coneObj.AddComponent<LineRenderer>();

        // Configurar propiedades del LineRenderer
        coneRenderer.startWidth = coneWidth;
        coneRenderer.endWidth = coneWidth * 0.5f;
        coneRenderer.material = new Material(Shader.Find("Sprites/Default"));
        coneRenderer.startColor = coneColor;
        coneRenderer.endColor = new Color(coneColor.r, coneColor.g, coneColor.b, 0);
        coneRenderer.useWorldSpace = true;
        coneRenderer.loop = true;
        coneRenderer.enabled = false;

        // Asegurar que el cono se vea por encima de todo
        coneRenderer.sortingLayerName = "Player";
        coneRenderer.sortingOrder = 100;
    }

    void Update()
    {
        // Actualizar dirección del disparo
        UpdateShotDirection();

        // Detectar clic izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            isShooting = true;
            ActivateCone();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
            DeactivateCone();
        }

        // Mantener el cono activo mientras se dispara
        if (isShooting)
        {
            UpdateCone();
            ApplyConeDamage();
        }

        // Actualizar temporizador del cono
        if (coneActive)
        {
            coneTimer -= Time.deltaTime;
            if (coneTimer <= 0f)
            {
                DeactivateCone();
            }
        }
    }

    void UpdateShotDirection()
    {
        // Calcular dirección del disparo (hacia el cursor)
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane;
        Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(mousePos);

        // Determinar punto de origen del disparo
        Vector3 fireOrigin = gunEndPoint != null ? gunEndPoint.position : transform.position;
        shotDirection = (worldMousePos - fireOrigin).normalized;
    }

    void ActivateCone()
    {
        coneActive = true;
        coneTimer = coneDuration;
        coneRenderer.enabled = true;

        if (coneLight != null)
        {
            coneLight.enabled = true;
        }
    }

    void DeactivateCone()
    {
        coneActive = false;
        coneRenderer.enabled = false;

        if (coneLight != null)
        {
            coneLight.enabled = false;
        }
    }

    void UpdateCone()
    {
        // Determinar punto de origen del disparo
        Vector3 fireOrigin = gunEndPoint != null ? gunEndPoint.position : transform.position;

        // Calcular punto final del cono
        coneEndPoint = fireOrigin + shotDirection * fixedDistance;

        // Verificar colisiones con obstáculos
        RaycastHit2D hit = Physics2D.Raycast(
            fireOrigin,
            shotDirection,
            fixedDistance,
            obstacleLayer
        );

        if (hit.collider != null)
        {
            coneEndPoint = hit.point;
        }

        // Calcular puntos del cono
        Vector3[] conePoints = CalculateConePoints(fireOrigin, coneEndPoint);

        // Actualizar el renderer del cono
        coneRenderer.positionCount = conePoints.Length;
        coneRenderer.SetPositions(conePoints);

        // Actualizar la luz direccional
        if (coneLight != null)
        {
            coneLight.transform.position = fireOrigin;
            coneLight.transform.rotation = Quaternion.LookRotation(Vector3.forward, shotDirection);
        }

        // Visualizar rayos de límite
        if (showConeRays)
        {
            VisualizeConeLimits(fireOrigin);
        }
    }

    Vector3[] CalculateConePoints(Vector3 origin, Vector3 endPoint)
    {
        // Calcular direcciones de los bordes del cono
        float halfAngle = coneAngle * 0.5f * Mathf.Deg2Rad;
        Vector3 leftDir = Quaternion.Euler(0, 0, halfAngle) * shotDirection;
        Vector3 rightDir = Quaternion.Euler(0, 0, -halfAngle) * shotDirection;

        // Calcular puntos finales
        Vector3 leftEnd = origin + leftDir * fixedDistance;
        Vector3 rightEnd = origin + rightDir * fixedDistance;

        // Verificar colisiones con obstáculos para los bordes
        RaycastHit2D leftHit = Physics2D.Raycast(origin, leftDir, fixedDistance, obstacleLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(origin, rightDir, fixedDistance, obstacleLayer);

        if (leftHit.collider != null) leftEnd = leftHit.point;
        if (rightHit.collider != null) rightEnd = rightHit.point;

        // Crear puntos para el cono (forma triangular)
        return new Vector3[] { origin, leftEnd, endPoint, rightEnd, origin };
    }

    void VisualizeConeLimits(Vector3 origin)
    {
        // Calcular direcciones de los bordes
        float halfAngle = coneAngle * 0.5f * Mathf.Deg2Rad;
        Vector3 leftDir = Quaternion.Euler(0, 0, halfAngle) * shotDirection;
        Vector3 rightDir = Quaternion.Euler(0, 0, -halfAngle) * shotDirection;

        // Visualizar rayo izquierdo
        Debug.DrawRay(origin, leftDir * fixedDistance, Color.yellow, 0.1f);

        // Visualizar rayo derecho
        Debug.DrawRay(origin, rightDir * fixedDistance, Color.yellow, 0.1f);

        // Visualizar rayo central
        Debug.DrawRay(origin, shotDirection * fixedDistance, Color.red, 0.1f);
    }

    void ApplyConeDamage()
    {
        Vector3 fireOrigin = gunEndPoint != null ? gunEndPoint.position : transform.position;

        // Detectar todos los enemigos dentro del cono
        Collider2D[] hits = Physics2D.OverlapCircleAll(fireOrigin, fixedDistance, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            // Verificar si el enemigo está dentro del cono
            if (IsInCone(fireOrigin, hit.transform.position))
            {
                // Verificar línea de visión (sin obstáculos)
                Vector3 directionToEnemy = (hit.transform.position - fireOrigin).normalized;
                float distanceToEnemy = Vector3.Distance(fireOrigin, hit.transform.position);

                RaycastHit2D obstacleHit = Physics2D.Raycast(
                    fireOrigin,
                    directionToEnemy,
                    distanceToEnemy,
                    obstacleLayer
                );

                // Si no hay obstáculos, aplicar daño
                if (obstacleHit.collider == null)
                {
                    EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        float damage = damagePerSecond * Time.deltaTime;
                        enemyHealth.TakeDamage((int)damage);
                        enemyHealth.ShowHealthBar();
                    }
                }
            }
        }
    }

    bool IsInCone(Vector3 origin, Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - origin).normalized;
        float angle = Vector3.Angle(shotDirection, directionToTarget);
        float distance = Vector3.Distance(origin, targetPosition);

        return angle <= coneAngle * 0.5f && distance <= fixedDistance;
    }

    // Dibujar Gizmos para depuración
    void OnDrawGizmosSelected()
    {
        if (gunEndPoint != null)
        {
            Vector3 fireOrigin = gunEndPoint.position;

            // Dibujar el cono
            float halfAngle = coneAngle * 0.5f * Mathf.Deg2Rad;
            Vector3 leftDir = Quaternion.Euler(0, 0, halfAngle) * shotDirection;
            Vector3 rightDir = Quaternion.Euler(0, 0, -halfAngle) * shotDirection;

            Gizmos.color = new Color(coneColor.r, coneColor.g, coneColor.b, 0.3f);
            Gizmos.DrawLine(fireOrigin, fireOrigin + leftDir * fixedDistance);
            Gizmos.DrawLine(fireOrigin, fireOrigin + rightDir * fixedDistance);
            Gizmos.DrawLine(fireOrigin + leftDir * fixedDistance, fireOrigin + shotDirection * fixedDistance);
            Gizmos.DrawLine(fireOrigin + shotDirection * fixedDistance, fireOrigin + rightDir * fixedDistance);

            // Dibujar arco del cono
            DrawConeArc(fireOrigin);
        }
    }

    void DrawConeArc(Vector3 origin)
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Vector3 startDir = Quaternion.Euler(0, 0, coneAngle * 0.5f) * shotDirection;

        const int segments = 20;
        Vector3 prevPoint = origin + startDir * fixedDistance;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -coneAngle * 0.5f + (coneAngle * i / segments);
            Vector3 dir = Quaternion.Euler(0, 0, angle) * shotDirection;
            Vector3 newPoint = origin + dir * fixedDistance;

            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}