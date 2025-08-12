using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public int maxEnemies = 10;
    public float spawnRadius = 10f;
    public LayerMask obstacleLayer;

    private float spawnTimer;
    private int currentEnemyCount;

    void Start()
    {
        spawnTimer = spawnInterval;
        currentEnemyCount = 0;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        
        if (spawnTimer <= 0 && currentEnemyCount < maxEnemies)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        // Buscar posición válida para spawnear
        Vector2 spawnPosition = FindValidSpawnPosition();
        
        if (spawnPosition != Vector2.zero)
        {
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemyCount++;
        }
    }

    Vector2 FindValidSpawnPosition()
    {
        Vector2 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        
        // Intentar encontrar posición válida (máx 10 intentos)
        for (int i = 0; i < 10; i++)
        {
            // Posición aleatoria en un círculo alrededor del jugador
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 spawnPos = playerPosition + randomDirection * spawnRadius;
            
            // Verificar si hay colisión con obstáculos
            if (!Physics2D.OverlapCircle(spawnPos, 0.5f, obstacleLayer))
            {
                // Verificar línea de visión al jugador
                RaycastHit2D hit = Physics2D.Raycast(spawnPos, playerPosition - spawnPos, 
                    Vector2.Distance(spawnPos, playerPosition), obstacleLayer);
                
                if (!hit.collider)
                {
                    return spawnPos;
                }
            }
        }
        
        return Vector2.zero;
    }

    // Método para que los enemigos notifiquen cuando son destruidos
    public void EnemyDestroyed()
    {
        currentEnemyCount--;
    }
}