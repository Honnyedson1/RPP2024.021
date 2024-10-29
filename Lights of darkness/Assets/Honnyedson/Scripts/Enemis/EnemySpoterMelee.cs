using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float followDistance = 5f; // Distância máxima para seguir o jogador
    public float attackDistance = 1.5f; // Distância para atacar
    public float yTolerance = 0.5f; // Tolerância no eixo Y para seguir o jogador
    public float attackCooldown = 1f; // Tempo entre ataques
    public float moveSpeed = 10f; // Velocidade de movimento do inimigo
    public float attackDelay = 0.1f; // Tempo de atraso antes de atacar
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

        if (!isdead)
        {
            if (player != null)
            {
                float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x); // Distância no eixo X
                float yDifference = Mathf.Abs(player.position.y - transform.position.y); // Diferença no eixo Y

                // Verifica se o inimigo está dentro da distância de seguir no eixo X
                if (!isAttacking && distanceToPlayer <= followDistance && yDifference <= yTolerance)
                {
                    FollowPlayerOnXAxis();
                }
                else
                {
                    // Se o jogador sair da tolerância, o inimigo pode passar direto
                    Debug.Log("Jogador saiu do campo de visão do inimigo.");
                }
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

    private void FollowPlayerOnXAxis()
    {
        // Move apenas no eixo X em direção ao jogador, mantendo a posição Y constante
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x); // Distância no eixo X

        // Se estiver na distância de ataque, ataque
        if (distanceToPlayer <= attackDistance)
        {
            StartCoroutine(PrepareAttack());
        }
    }

    private IEnumerator PrepareAttack()
    {
        isAttacking = true; // Indica que o inimigo está se preparando para atacar

        // Pausar antes do ataque
        yield return new WaitForSeconds(attackDelay);

        // Verificar se o jogador ainda está no alcance para o ataque
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float yDifference = Mathf.Abs(player.position.y - transform.position.y);

        // Verifica se o jogador está pulando
        PlayerController playerController = player.GetComponent<PlayerController>();

        if (distanceToPlayer <= attackDistance && yDifference <= yTolerance)
        {
            // Atacar o jogador se ele ainda estiver dentro do alcance
            AttackPlayer();
        }
        else
        {
            // Cancelar o ataque se o jogador tiver saído da área de ataque ou está pulando
            Debug.Log("Jogador saiu da área de ataque ou está pulando. Ataque cancelado.");
        }

        isAttacking = false; // Ataque concluído ou cancelado
    }

    private void AttackPlayer()
    {
        // O inimigo causa dano instantaneamente ao jogador
        PlayerController playerHealth = player.GetComponent<PlayerController>();
        if (playerHealth != null)
        {
            playerHealth.TakeDmg(100); // O jogador morre instantaneamente
            Debug.Log("Inimigo atacou o jogador e causou hit kill!");
        }
    }

    // Detectar colisão com o jogador
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null) // Apenas cause dano se o jogador não estiver pulando
            {
                AttackPlayer(); // Causa dano ao jogador
            }
            else
            {
                Debug.Log("Jogador pulou e passou pelo inimigo.");
                // Aqui você pode adicionar a lógica adicional que quiser quando o jogador pula.
            }
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
