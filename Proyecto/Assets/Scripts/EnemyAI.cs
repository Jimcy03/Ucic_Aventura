using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float searchSpeed = 1f;
    public float searchRadius = 3f;
    public float searchInterval = 2f;
    public float searchChangeDirectionChance = 0.5f;

    [Header("Combat Settings")]
    public float detectionRadius = 5f;
    public float attackRange = 1f;
    public int damage = 10;
    public float attackCooldown = 1f;

    private Transform playerTarget;
    private Rigidbody2D rb;
    private float attackTimer;
    private Vector2 searchDirection;
    private float searchTimer;
    private bool isSearching = false;
    private Vector2 lastKnownPosition;
    private bool hasReachedLastPosition = true;

    // Asignar el objetivo del jugador
    public void SetPlayerTarget(Transform target)
    {
        playerTarget = target;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackTimer = attackCooldown;
        searchTimer = searchInterval;

        // Congelar rotación
        rb.freezeRotation = true;

        // Si no se asignó el objetivo, intentar encontrarlo
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
        }

        // Iniciar con una dirección aleatoria
        searchDirection = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        if (playerTarget == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (attackTimer > 0) attackTimer -= Time.deltaTime;

        // Comportamiento de búsqueda cuando no detecta al jugador
        if (distanceToPlayer > detectionRadius)
        {
            HandleSearchBehavior();
        }
        else
        {
            isSearching = false;
            ChasePlayer();

            if (distanceToPlayer <= attackRange && attackTimer <= 0)
            {
                AttackPlayer();
            }
        }
    }

    void HandleSearchBehavior()
    {
        // Si no está buscando, comenzar a buscar
        if (!isSearching)
        {
            isSearching = true;
            lastKnownPosition = playerTarget.position;
            hasReachedLastPosition = false;
        }

        // Si no ha llegado a la última posición conocida, moverse hacia ella
        if (!hasReachedLastPosition)
        {
            Vector2 direction = (lastKnownPosition - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * searchSpeed;

            // Comprobar si ha llegado
            if (Vector2.Distance(transform.position, lastKnownPosition) < 0.5f)
            {
                hasReachedLastPosition = true;
                StartCoroutine(SearchRoutine());
            }
        }
    }

    IEnumerator SearchRoutine()
    {
        while (isSearching)
        {
            // Cambiar dirección aleatoriamente
            if (Random.value < searchChangeDirectionChance)
            {
                searchDirection = Random.insideUnitCircle.normalized;
            }

            // Mover en la dirección actual
            rb.linearVelocity = searchDirection * searchSpeed;

            // Esperar antes de cambiar de nuevo
            yield return new WaitForSeconds(searchInterval);
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (playerTarget.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    void AttackPlayer()
    {
        attackTimer = attackCooldown;

        PlayerHealth playerHealth = playerTarget.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Dibujar radios de detección y búsqueda
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Dibujar radio de búsqueda si está buscando
        if (isSearching)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, searchRadius);

            // Dibujar dirección de búsqueda
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + searchDirection * searchRadius);
        }
    }
}