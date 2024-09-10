using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject projectilePrefab; 
    public GameObject specialProjectilePrefab; 
    public Transform[] teleportPoints; 
    public float attackRange = 10f; 
    public float teleportCooldown = 5f; 
    public float attackCooldown = 3f; 
    public float specialAttackChance = 0.30f; 
    public float detectionRadius = 5f; 
    public float projectileLifetime = 3f;

    private int Lifeboss = 2;
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
        teleportTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;
        if (teleportTimer <= 0f)
        {
            Teleport();
            teleportTimer = teleportCooldown; 
        }

        // Controle do ataque
        if (playerInRange && attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = attackCooldown; 
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
        GameObject projectileToShoot = Random.value < specialAttackChance ? specialProjectilePrefab : projectilePrefab;
        GameObject projectile = Instantiate(projectileToShoot, transform.position, Quaternion.identity);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * 15f;
            Destroy(projectile, projectileLifetime);
        }
    }
    public void TakeDamage(int damage)
    {
        Lifeboss -= damage;
        if (Lifeboss <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("O inimigo morreu!");
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
