using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float followDistance = 5f; // Distância máxima para seguir o jogador
    public float attackDistance = 1.5f; // Distância para atacar
    public float yTolerance = 0.5f; // Tolerância no eixo Y para o ataque
    public float attackCooldown = 1f; // Tempo entre ataques
    public float moveSpeed = 5f; // Velocidade de movimento do inimigo
    public float attackDelay = 0.4f; // Tempo de atraso antes de atacar
    public float spacingDistance = 1f; // Distância mínima para manter dos outros inimigos
    private int vida = 4;
    private float lastAttackTime;
    private Transform player;
    private bool isAttacking = false; // Controla se o inimigo está no meio de um ataque
    private bool isdead;

    private void Start()
    {
        // Busca o jogador pela tag "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (InimigoMovimentoLinear.PlayerVivo == false || InimigoRaycastVisao.PlayerVivo == false)
        {
            Debug.Log("Morreu");
            Destroy(this.gameObject);
        }
        if (isdead == false && player != null)
        {
            float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x); // Distância no eixo X
            float yDifference = Mathf.Abs(player.position.y - transform.position.y); // Diferença no eixo Y
            
            // Verifica se o inimigo está dentro da distância de seguir no eixo X e dentro da tolerância no eixo Y
            if (!isAttacking && distanceToPlayer <= followDistance)
            {
                FollowPlayerOnXAxis(distanceToPlayer, yDifference);
            }
        }
    }

    public void takedmg(int dmg)
    {
        vida -= dmg;
        if (vida <= 0)
        {
            Destroy(gameObject, 2f);
            isdead = true;
        }
    }

    private void FollowPlayerOnXAxis(float distanceToPlayer, float yDifference)
    {
        // Check for other enemies in front
        if (!IsBlockedByAnotherEnemy())
        {
            if (distanceToPlayer > attackDistance || yDifference > yTolerance)
            {
                // Move apenas no eixo X em direção ao jogador, mantendo a posição Y constante
                Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                // Se estiver na distância de ataque e dentro da tolerância de altura, atacar
                StartCoroutine(PrepareAttack());
            }
        }
    }

    private bool IsBlockedByAnotherEnemy()
    {
        // Raycast para verificar se há um inimigo na frente
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(player.position.x - transform.position.x), spacingDistance, LayerMask.GetMask("Enemy"));

        // Retorna true se encontrar outro inimigo
        return hit.collider != null && hit.collider.gameObject != this.gameObject;
    }

    private IEnumerator PrepareAttack()
    {
        isAttacking = true; // Indica que o inimigo está se preparando para atacar

        // Pausar antes do ataque
        yield return new WaitForSeconds(attackDelay);

        // Verificar se o jogador ainda está no alcance para o ataque
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float yDifference = Mathf.Abs(player.position.y - transform.position.y);

        if (distanceToPlayer <= attackDistance && yDifference <= yTolerance)
        {
            // Atacar o jogador se ele ainda estiver dentro do alcance
            AttackPlayer();
        }
        else
        {
            // Cancelar o ataque se o jogador tiver saído da área de ataque
            Debug.Log("Jogador saiu da área de ataque. Ataque cancelado.");
        }

        isAttacking = false; // Ataque concluído ou cancelado
    }

    private void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        // Espera 0.4 segundos antes de realmente causar o dano (pausa antes do ataque)
        yield return new WaitForSeconds(0.4f);

        // Chama a função TakeDamage do jogador se ele ainda estiver no alcance
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float yDifference = Mathf.Abs(player.position.y - transform.position.y);

        if (distanceToPlayer <= attackDistance && yDifference <= yTolerance)
        {
            PlayerController playerHealth = player.GetComponent<PlayerController>();
            if (playerHealth != null)
            {
                playerHealth.TakeDmg(1); // O valor do dano pode ser ajustado
                Debug.Log("Inimigo atacou o jogador!");
            }
        }
        else
        {
            Debug.Log("Jogador saiu do alcance antes do ataque finalizado. Dano cancelado.");
        }
    }

    // Visualizar o alcance de ataque usando Gizmos
    private void OnDrawGizmosSelected()
    {
        // Desenhar o raio de ataque no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        // Desenhar o raio de detecção do jogador no editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followDistance);
    }
}
