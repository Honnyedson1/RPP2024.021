using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private BossHealthSlider healthSlider;
    public GameObject projectilePrefab; 
    public GameObject specialProjectilePrefab; 
    public GameObject enemyPrefab;  // Referência ao prefab do inimigo
    public Transform[] teleportPoints; 
    public float attackRange = 10f; 
    public float teleportCooldown = 5f; 
    public float attackCooldown = 3f; 
    public float specialAttackChance = 0.30f; 
    public float detectionRadius = 5f; 
    public float projectileLifetime = 3f;
    private bool phaseTwoActivated = false; 
    public Animator bossAnimator;
    public int Lifeboss = 25;
    private Transform player;
    private float teleportTimer;
    private float attackTimer;
    public bool playerInRange = false;
    public LayerMask playerLayer;
    private bool Isdead = false;

    // Para o movimento suave
    private float moveTimer;
    public float moveInterval = 1.5f; // Intervalo entre movimentos suaves

    // Para o spawn de inimigos
    private float spawnTimer;
    public float spawnInterval = 40f; // Intervalo para spawnar inimigos (agora 40 segundos)

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        healthSlider = GetComponent<BossHealthSlider>();

        if (player == null)
        {
            Debug.LogError("Player não encontrado!");
            return;
        }

        if (healthSlider == null)
        {
            Debug.LogError("BossHealthSlider não encontrado!");
            return;
        }

        teleportTimer = teleportCooldown;
        attackTimer = attackCooldown;
        moveTimer = moveInterval;
        spawnTimer = spawnInterval;

        bossAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        if (player == null) return;

        // Sempre vira o Boss para o jogador
        FacePlayer();

        // Checa se está na metade da vida e ativa a fase 2
        if (Lifeboss <= 12 && !phaseTwoActivated)
        {
            ActivatePhaseTwo(); // Ativa a fase 2
        }

        if (!Isdead)
        {
            teleportTimer -= Time.deltaTime;
            attackTimer -= Time.deltaTime;
            moveTimer -= Time.deltaTime;
            spawnTimer -= Time.deltaTime;

            // Verifica manualmente se o jogador está na área de detecção
            playerInRange = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

            if (teleportTimer <= 0f)
            {
                Teleport();
                teleportTimer = teleportCooldown; 
            }

            if (playerInRange && attackTimer <= 0f)
            {
                StartCoroutine(PerformAttack());
                attackTimer = attackCooldown; 
            }

            // Movimentação suave do boss
            if (moveTimer <= 0f)
            {
                SmoothMovement();
                moveTimer = moveInterval;  // Reseta o timer de movimento
            }

            // Spawna inimigos no segundo estágio
            if (phaseTwoActivated && spawnTimer <= 0f)
            {
                SpawnEnemies();
                spawnTimer = spawnInterval;  // Reseta o timer de spawn
            }
        }

        // Verifica se a vida do jogador chegou a 0 e reseta tudo
        if (GameManager.Instance.Life <= 0)
        {
            ResetGame();
        }
    }

    void ResetGame()
    {
        Lifeboss = 40;
        phaseTwoActivated = false;
        specialAttackChance = 0.30f;
        teleportCooldown = 7f;
        attackCooldown = 3f;
        spawnInterval = 10f;

        InimigoController.DestruirTodosInimigos();
        transform.position = new Vector2(0, 0);

        teleportTimer = teleportCooldown;
        attackTimer = attackCooldown;
        moveTimer = moveInterval;
        spawnTimer = spawnInterval;

        healthSlider.ResetSlider(); // Reseta o slider de vida

        Debug.Log("O jogador morreu. O Boss e inimigos foram resetados.");
    }

    void Teleport()
    {
        if (teleportPoints.Length == 0) return;

        int randomIndex = Random.Range(0, teleportPoints.Length);
        transform.position = teleportPoints[randomIndex].position;
    }

    IEnumerator PerformAttack()
    {
        if (bossAnimator != null)
        {
            GameObject projectile;
            Rigidbody2D rb;

            if (Random.value < specialAttackChance) // Ataque especial
            {
                bossAnimator.SetTrigger("AttackSpecial");
                Debug.Log("Boss atacando com ataque especial!");
                yield return new WaitForSeconds(0.25f);
                projectile = Instantiate(specialProjectilePrefab, transform.position, Quaternion.identity);
            }
            else // Ataque comum
            {
                bossAnimator.SetTrigger("AttackCommon");
                Debug.Log("Boss atacando com ataque comum!");
                yield return new WaitForSeconds(0.25f);
                projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            }

            // Configuração do projétil
            rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Calcula a direção exata para o jogador
                Vector2 direction = (player.position - transform.position).normalized;

                // Define a velocidade do projétil
                rb.velocity = direction * 15f;

                // Ajuste a rotação do projétil (se necessário)
                AdjustProjectileRotation(projectile, direction);

                // Destroi o projétil após um tempo
                Destroy(projectile, projectileLifetime);
            }
        }
    }
    void AdjustProjectileRotation(GameObject projectile, Vector2 direction)
    {
        // Se o projétil estiver indo para a direita, rotaciona para 180
        if (direction.x > 0)
        {
            projectile.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            projectile.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void FacePlayer()
    {
        // Calcula a direção do jogador em relação ao Boss
        Vector2 direction = (player.position - transform.position).normalized;

        // Se o jogador estiver à direita do Boss (direção positiva no eixo X)
        if (direction.x > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0); // Olhar para a direita
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0); // Olhar para a esquerda
        }
    }



    void SmoothMovement()
    {
        // Movimentação suave, com deslocamentos pequenos aleatórios
        float moveX = Random.Range(-0.5f, 0.5f); // Pequeno deslocamento no eixo X
        float moveY = Random.Range(-0.5f, 0.5f); // Pequeno deslocamento no eixo Y
        Vector2 movement = new Vector2(moveX, moveY);
        transform.Translate(movement);  // Movimenta suavemente o boss
    }

    public Transform spawnPoint; // Ponto de spawn definido no Editor

    void SpawnEnemies()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("Ponto de spawn não definido! Por favor, atribua um ponto de spawn no Inspector.");
            return;
        }

        Debug.Log("Spawn de inimigos no ponto designado!");
        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }

    void ActivatePhaseTwo()
    {
        phaseTwoActivated = true;
        specialAttackChance = 0.6f; // Aumenta chance de ataque especial
        teleportCooldown = 3f; // Boss teleporta com mais frequência
        attackCooldown = 2f; // Ataques ficam mais rápidos
        spawnInterval = 40f;  // A cada 40 segundos, spawn de inimigos
        Debug.Log("O Boss ativou a fase 2!");
    }

    public void TakeDamage(int damage)
    {
        Lifeboss -= damage;
        if (Lifeboss <= 0)
        {
            Die();
        }
        else
        {
            bossAnimator.SetTrigger("Hit");
            healthSlider.healthSlider.value = Lifeboss; // Atualiza a barra
        }
    }

    private void Die()
    {
        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("Dead");  // Trigger para animação de morte
        }

        StartCoroutine(EndLevelAfterDelay(5f)); // Espera 5 segundos antes de trocar a fase
    }

    private IEnumerator EndLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Lógica para terminar a fase (sem carregar a cena)
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}