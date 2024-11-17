using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool phaseTwoActivated = false; 

    public int Lifeboss = 25;
    private Transform player;
    private float teleportTimer;
    private float attackTimer;
    private bool playerInRange = false;
    public LayerMask playerLayer;
    private bool Isdead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player não encontrado!");
            return;
        }
        teleportTimer = teleportCooldown;
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (player == null) return;

        // Checa se está na metade da vida e ativa uma nova mecânica
        if (Lifeboss <= 12 && !phaseTwoActivated)
        {
            ActivatePhaseTwo();
        }

        if (Isdead == false)
        {
            teleportTimer -= Time.deltaTime;
            attackTimer -= Time.deltaTime;
        
            // Verifica manualmente se o jogador está na área de detecção
            playerInRange = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

            if (teleportTimer <= 0f)
            {
                Teleport();
                teleportTimer = teleportCooldown; 
            }

            if (playerInRange && attackTimer <= 0f)
            {
                PerformAttack();
                attackTimer = attackCooldown; 
            }
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
        Debug.Log("Boss atacando!");
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
    
    void ActivatePhaseTwo()
    {
        phaseTwoActivated = true;
        specialAttackChance = 0.6f; // Aumenta chance de ataque especial
        teleportCooldown = 3f; // Boss teleporta com mais frequência
        attackCooldown = 2f; // Ataques ficam mais rápidos
        Debug.Log("O Boss ativou a fase 2!");
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
        StartCoroutine(EndLevelAfterDelay(5f)); // Espera 5 segundos antes de trocar a fase
    }

    private IEnumerator EndLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Carrega a próxima cena
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
