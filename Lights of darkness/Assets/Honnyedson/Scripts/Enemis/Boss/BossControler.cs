using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
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
    private bool playerInRange = false;
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
        if (player == null)
        {
            Debug.LogError("Player não encontrado!");
            return;
        }
        teleportTimer = teleportCooldown;
        attackTimer = attackCooldown;
        moveTimer = moveInterval;
        spawnTimer = spawnInterval;

        // Inicializando o Animator
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
            Debug.Log("Aa");
        }

        // Verifica se o Boss está vivo
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

            // Só realiza o ataque se o Boss estiver vivo
            if (Lifeboss > 0 && playerInRange && attackTimer <= 0f)
            {
                PerformAttack();
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
        else
        {
            // Se o Boss morreu, ele não faz mais ações de ataque ou movimento
            // Aqui, o Boss pode fazer uma animação de cair no chão
            if (bossAnimator != null && !bossAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
            {
                bossAnimator.SetTrigger("Dead");  // Trigger para animação de morte
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
        // Resetar a vida do boss e a lógica dele
        Lifeboss = 25;
        phaseTwoActivated = false;
        specialAttackChance = 0.30f;
        teleportCooldown = 5f;
        attackCooldown = 3f;
        spawnInterval = 40f;  // Resetando o intervalo de spawn para 40 segundos

        // Destruir todos os inimigos na cena
        InimigoController.DestruirTodosInimigos();

        // Resetar a posição do Boss para o início (se necessário)
        transform.position = new Vector2(0, 0); // Exemplo de resetar a posição, ajuste conforme necessário

        // Resetar timers
        teleportTimer = teleportCooldown;
        attackTimer = attackCooldown;
        moveTimer = moveInterval;
        spawnTimer = spawnInterval;

        Debug.Log("O jogador morreu. O Boss e inimigos foram resetados.");
    }

    void Teleport()
    {
        if (teleportPoints.Length == 0) return;

        int randomIndex = Random.Range(0, teleportPoints.Length);
        transform.position = teleportPoints[randomIndex].position;
    }

    void PerformAttack()
    {
        // Verifica se o Boss ainda está vivo antes de atacar
        if (Lifeboss <= 0) return;

        if (bossAnimator != null)
        {
            if (Random.value < specialAttackChance) // Se o ataque for especial
            {
                bossAnimator.SetTrigger("AttackSpecial");  // Trigger para animação de ataque especial
                Debug.Log("Boss atacando com ataque especial!");
        
                // Executando o ataque especial
                GameObject projectile = Instantiate(specialProjectilePrefab, transform.position, Quaternion.identity);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (player.position - transform.position).normalized;
                    rb.velocity = direction * 15f;
                    Destroy(projectile, projectileLifetime);
                }
            }
            else // Caso contrário, ataque comum
            {
                bossAnimator.SetTrigger("AttackCommon");  // Trigger para animação de ataque comum
                Debug.Log("Boss atacando com ataque comum!");
        
                // Executando o ataque comum
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (player.position - transform.position).normalized;
                    direction += new Vector2(0f, 0.2f); // Ajuste para cima (valor fixo)
                    direction = direction.normalized;

                    rb.velocity = direction * 15f;
                    Destroy(projectile, projectileLifetime);
                }
            }
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

    void SpawnEnemies()
    {
        Debug.Log("Spawn de inimigos!");
        // Spawn de um inimigo em uma posição próxima do boss
        Vector2 spawnPosition = (Vector2)transform.position + new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
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
            bossAnimator.SetTrigger("Hit");  // Trigger para animação de hit (dano)
        }
    }

    private void Die()
    {
        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("Dead");  // Trigger para animação de morte
        }
        Isdead = true; // Marca o Boss como morto
        // O Boss para de atacar, se mover ou fazer qualquer ação depois da morte

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
