using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float detectionRadius = 5f;
    public float attackRange = 1f;
    public int damage = 10;
    public float attackCooldown = 1f;

    private Transform player;
    private Rigidbody2D rb;
    private float attackTimer;
    private Vector2 lastKnownPosition;
    private bool isChasing;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Actualizar temporizador de ataque
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // Detección del jugador
        if (distanceToPlayer <= detectionRadius)
        {
            isChasing = true;
            lastKnownPosition = player.position;

            // Perseguir al jugador
            ChasePlayer();

            // Atacar si está en rango
            if (distanceToPlayer <= attackRange && attackTimer <= 0)
            {
                AttackPlayer();
            }
        }
        else
        {
            // Si ya estaba persiguiendo pero perdió de vista
            if (isChasing)
            {
                // Ir a la última posición conocida
                Vector2 direction = (lastKnownPosition - (Vector2)transform.position).normalized;
                rb.linearVelocity = direction * moveSpeed;

                // Rotar hacia la dirección de movimiento
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Slerp(transform.rotation, 
                    Quaternion.Euler(0, 0, angle), rotationSpeed * Time.deltaTime);

                // Si llegó a la última posición conocida, dejar de perseguir
                if (Vector2.Distance(transform.position, lastKnownPosition) < 0.5f)
                {
                    isChasing = false;
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // Rotar hacia el jugador
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Slerp(transform.rotation, 
            Quaternion.Euler(0, 0, angle), rotationSpeed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        // Reiniciar temporizador
        attackTimer = attackCooldown;
        
        // Aplicar daño al jugador
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Dibujar radios de detección y ataque
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}