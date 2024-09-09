using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections; // Necessário para utilizar Coroutines

public class DodgeEnemy : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float dodgeProbability = 0.7f; // Probabilidade de desviar das flechas
    public float followRange = 10f; // Distância máxima para seguir o jogador
    public Transform areaLimit;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb; // Referência ao Rigidbody2D
    private float lastAttackTime;
    public int Life = 3;
    public event Action OnDeath; 
    private Coroutine attackCoroutine; // Referência à corrotina de ataque ativa

    private void Start()
    {
        // Encontre o jogador na cena, caso não tenha sido atribuído
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Certifique-se de que todos os componentes necessários sejam encontrados
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>(); // Inicializa o Rigidbody2D

        // Verifica se areaLimit foi configurado
        if (areaLimit == null)
        {
            Debug.LogError("Área de Limite não configurada para o inimigo.");
        }
    }

    private void Update()
    {
        if (player != null) // Verifica se o jogador existe
        {
            float distanceToPlayer = Vector2.Distance(player.position, transform.position); // Calcula a distância ao jogador
            
            if (distanceToPlayer <= followRange) // Verifica se o jogador está dentro da faixa de seguimento
            {
                if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
                {
                    if (attackCoroutine == null) // Inicia a corrotina de ataque se não houver nenhuma em execução
                    {
                        attackCoroutine = StartCoroutine(AttackCoroutine());
                    }
                }
                else
                {
                    MoveTowardsPlayer();
                }
            
                // Atualiza a orientação do sprite com base na direção do jogador
                spriteRenderer.flipX = (player.position.x < transform.position.x);  
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        if (Vector2.Distance(player.position, transform.position) > attackRange)
        {
            if (Random.value > dodgeProbability) // Chance de desviar
            {
                // Apenas move no eixo X, mantendo o eixo Y inalterado para não "voar"
                rb.velocity = new Vector2(directionToPlayer.x * speed, rb.velocity.y); 
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y); // Para de mover se decidir desviar
            }
        }
        else
        {
            rb.velocity = Vector2.zero; // Para completamente se dentro da distância de ataque
        }
    }

    private IEnumerator AttackCoroutine()
    {
        // Inicia o ataque
        Debug.Log("Preparando ataque...");
        yield return new WaitForSeconds(0.3f); // Espera por 0.3 segundos

        float distanceToPlayer = Vector2.Distance(player.position, transform.position); // Recalcula a distância ao jogador

        if (distanceToPlayer <= attackRange) // Verifica se o jogador ainda está no alcance de ataque
        {
            ApplyDamage();
        }
        else
        {
            Debug.Log("Ataque falhou, jogador fora do alcance.");
        }

        // Finaliza a corrotina e registra o tempo do último ataque
        attackCoroutine = null;
        lastAttackTime = Time.time;
    }

    private void ApplyDamage()
    {
        GameManager.Instance.Life--; // Aplica dano ao jogador
        Debug.Log("DodgeEnemy atacou!");
    }

    public void TakeDamage(int damage)
    {
        Life -= damage;
        if (Life <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("O DodgeEnemy morreu!");
        OnDeath?.Invoke(); // Dispara o evento de morte
        Destroy(gameObject);
    }

    public void OnArrowHit()
    {
        if (Random.value > dodgeProbability) // Chance de desviar
        {
            TakeDamage(1); // Exemplo de dano
        }
        else
        {
            Debug.Log("DodgeEnemy desviou da flecha!");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green; 
        Gizmos.DrawWireSphere(transform.position, followRange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue; 
        Gizmos.DrawWireSphere(transform.position, followRange);
    }
}
