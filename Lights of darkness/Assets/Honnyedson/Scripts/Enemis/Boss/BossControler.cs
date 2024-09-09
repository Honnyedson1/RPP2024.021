using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab do projétil normal
    public GameObject specialProjectilePrefab; // Prefab do projétil especial
    public Transform[] teleportPoints; // Pontos onde o boss pode teletransportar
    public float attackRange = 10f; // Distância do ataque
    public float teleportCooldown = 5f; // Tempo entre teletransportes
    public float attackCooldown = 3f; // Tempo entre ataques
    public float specialAttackChance = 0.30f; // Chance de ataque especial (aumentada para 30%)
    public float detectionRadius = 5f; // Raio de detecção do jogador
    public float projectileLifetime = 3f; // Duração do projétil antes de ser destruído

    private Transform player;
    private float teleportTimer;
    private float attackTimer;
    private bool playerInRange = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found. Ensure that the player object is tagged with 'Player'.");
            return;
        }
        teleportTimer = teleportCooldown;
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (player == null) return;

        // Atualiza os timers
        teleportTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;

        // Controle do teletransporte
        if (teleportTimer <= 0f)
        {
            Teleport();
            teleportTimer = teleportCooldown; // Reinicia o timer
        }

        // Controle do ataque
        if (playerInRange && attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = attackCooldown; // Reinicia o timer
        }
    }

    void Teleport()
    {
        if (teleportPoints.Length == 0) return;

        int randomIndex = Random.Range(0, teleportPoints.Length);
        transform.position = teleportPoints[randomIndex].position;
    }

    void PerformAttack()
    {
        if (projectilePrefab == null || specialProjectilePrefab == null)
        {
            Debug.LogWarning("Projectile prefabs are not assigned.");
            return;
        }

        GameObject projectileToShoot = Random.value < specialAttackChance ? specialProjectilePrefab : projectilePrefab;
        GameObject projectile = Instantiate(projectileToShoot, transform.position, Quaternion.identity);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * 15f; // Ajuste a velocidade conforme necessário

            // Destroi o projétil após 3 segundos
            Destroy(projectile, projectileLifetime);
        }
        else
        {
            Debug.LogWarning("Projectile does not have a Rigidbody2D component.");
        }
    }

    // Detecta o jogador entrando na área de detecção
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Detecta o jogador saindo da área de detecção
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    // Desenha gizmos na cena para visualizar a área de detecção
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
