using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int damage = 10;
    public float range = 20f;           // Distancia fija del disparo
    public float fireRate = 0.2f;        // Disparos por segundo
    public float rayDuration = 0.1f;     // Tiempo visible del rayo

    [Header("Visual Settings")]
    public Color rayColor = Color.red;
    public float rayWidth = 0.08f;
    public Transform gunEndPoint;

    [Header("Layer Settings")]
    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;

    [Header("Debug Settings")]
    public bool logHits = true;
    public bool logDamageValues = true;
    public bool logEnemyNames = true;

    private Camera mainCamera;
    private float nextFireTime;
    private Vector3 shotDirection;
    private bool isShooting = false;
    private LineRenderer rayRenderer;

    void Start()
    {
        mainCamera = Camera.main;
        CreateRayRenderer();
    }

    void CreateRayRenderer()
    {
        // Crear un nuevo objeto para el rayo
        GameObject rayObj = new GameObject("RayRenderer");
        rayObj.transform.SetParent(transform);
        rayRenderer = rayObj.AddComponent<LineRenderer>();

        // Configurar propiedades del LineRenderer
        rayRenderer.startWidth = rayWidth;
        rayRenderer.endWidth = rayWidth * 0.5f;
        rayRenderer.material = new Material(Shader.Find("Sprites/Default"));
        rayRenderer.startColor = rayColor;
        rayRenderer.endColor = new Color(rayColor.r, rayColor.g, rayColor.b, 0);
        rayRenderer.positionCount = 2;
        rayRenderer.enabled = false;

        // Asegurar que el rayo se vea por encima de todo
        rayRenderer.sortingLayerName = "Player";
        rayRenderer.sortingOrder = 100;
    }

    void Update()
    {
        // Actualizar dirección del disparo
        UpdateShotDirection();

        // Detectar clic izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            isShooting = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
        }

        // Disparar con clic o mantener presionado
        if (isShooting && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
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

    void Shoot()
    {
        // Determinar punto de origen del disparo
        Vector3 fireOrigin = gunEndPoint != null ? gunEndPoint.position : transform.position;

        // Calcular punto final del rayo
        Vector3 rayEndPoint = fireOrigin + shotDirection * range;

        // Comprobar colisiones con obstáculos
        RaycastHit2D obstacleHit = Physics2D.Raycast(
            fireOrigin,
            shotDirection,
            range,
            obstacleLayer
        );

        // Si hay obstáculo, ajustar punto final
        if (obstacleHit.collider != null)
        {
            rayEndPoint = obstacleHit.point;

            // Log de impacto con obstáculo
            if (logHits) Debug.Log($"Disparo impactó con obstáculo: {obstacleHit.collider.name}");
        }

        // Mostrar el rayo
        StartCoroutine(ShowRay(fireOrigin, rayEndPoint));

        // Comprobar colisiones con enemigos
        RaycastHit2D enemyHit = Physics2D.Raycast(
            fireOrigin,
            shotDirection,
            range,
            enemyLayer
        );

        // Aplicar daño si golpeó a un enemigo
        if (enemyHit.collider != null)
        {
            // Verificar si el enemigo está delante del obstáculo
            if (obstacleHit.collider == null || enemyHit.distance < obstacleHit.distance)
            {
                ProcessEnemyHit(enemyHit);
            }
        }
    }

    void ProcessEnemyHit(RaycastHit2D hit)
    {
        EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            enemyHealth.ShowHealthBar();

            // Generar log de impacto
            GenerateHitLog(hit.collider.gameObject);
        }
    }

    void GenerateHitLog(GameObject enemy)
    {
        if (!logHits) return;

        string logMessage = "Disparo impactó con enemigo";

        if (logEnemyNames)
        {
            logMessage += $": {enemy.name}";
        }

        if (logDamageValues)
        {
            logMessage += $". Daño aplicado: {damage}";
        }

        Debug.Log(logMessage);
    }

    IEnumerator ShowRay(Vector3 startPoint, Vector3 endPoint)
    {
        if (rayRenderer == null) yield break;

        rayRenderer.enabled = true;
        rayRenderer.SetPosition(0, startPoint);
        rayRenderer.SetPosition(1, endPoint);

        // Hacer el rayo más visible
        float elapsed = 0f;
        while (elapsed < rayDuration)
        {
            // Suavizar la desaparición
            float alpha = 1f - (elapsed / rayDuration);
            Color startColor = new Color(rayColor.r, rayColor.g, rayColor.b, alpha);
            Color endColor = new Color(rayColor.r, rayColor.g, rayColor.b, 0);

            rayRenderer.startColor = startColor;
            rayRenderer.endColor = endColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        rayRenderer.enabled = false;
    }

    // Dibujar Gizmos para depuración
    void OnDrawGizmos()
    {
        if (isShooting && gunEndPoint != null)
        {
            Vector3 fireOrigin = gunEndPoint.position;
            Vector3 rayEnd = fireOrigin + shotDirection * range;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(fireOrigin, rayEnd);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(rayEnd, 0.2f);
        }
    }
}