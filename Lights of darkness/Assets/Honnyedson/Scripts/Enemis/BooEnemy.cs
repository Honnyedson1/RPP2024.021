using UnityEngine;

public class BooEnemy : MonoBehaviour
{
    public Transform player;                     // Referência ao jogador
    public float speed = 2f;                     // Velocidade de movimento do Boo
    public float stopDistance = 2f;              // Distância mínima para parar de se mover
    public float attackRange = 1f;               // Distância de ataque
    public float attackCooldown = 1f;            // Tempo entre ataques
    public Transform areaLimit;                  // Área que limita o movimento do Boo

    private SpriteRenderer spriteRenderer;       // Referência ao SpriteRenderer para controlar a aparência do Boo
    private float lastAttackTime;                // Tempo do último ataque

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (IsPlayerInArea())
        {
            Vector3 directionToPlayer = player.position - transform.position; // Direção em relação ao jogador
            float distanceToPlayer = directionToPlayer.magnitude;            // Distância até o jogador

            bool isPlayerLooking = IsPlayerLooking(); // Verifica se o jogador está olhando para o Boo

            if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                Attack(); // Realiza o ataque
                lastAttackTime = Time.time; // Atualiza o tempo do último ataque
            }
            else if (!isPlayerLooking && distanceToPlayer > stopDistance)
            {
                // Move o Boo em direção ao jogador se o jogador não estiver olhando
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }

            // Inverte a direção do sprite do Boo com base na direção em relação ao jogador
            spriteRenderer.flipX = directionToPlayer.x < 0;
        }
    }

    private bool IsPlayerInArea()
    {
        // Verifica se o jogador está dentro da área limitada
        return areaLimit.GetComponent<Collider2D>().bounds.Contains(player.position);
    }

    private bool IsPlayerLooking()
    {
        // Direção em que o jogador está olhando (supondo que o jogador só pode olhar para a esquerda ou direita)
        Vector3 playerLookDirection = player.localScale.x > 0 ? Vector3.right : Vector3.left;

        // Verifica se a direção em que o jogador está olhando está alinhada com a direção até o Boo
        return Vector3.Dot(playerLookDirection, (transform.position - player.position).normalized) > 0;
    }

    private void Attack()
    {
        PlayerController._instance.health--;
        Debug.Log("Boo atacou!");
    }

    private void OnDrawGizmos()
    {
        if (areaLimit != null)
        {
            // Desenha a área de limite com Gizmos
            Gizmos.color = Color.green; // Cor para a área de limite
            Gizmos.DrawWireCube(areaLimit.position, areaLimit.localScale);
        }

        // Desenha a área de ataque com Gizmos
        Gizmos.color = Color.red; // Cor para a área de ataque
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha a área de limite quando o objeto está selecionado
        if (areaLimit != null)
        {
            Gizmos.color = Color.yellow; // Cor para a área de limite quando selecionado
            Gizmos.DrawWireCube(areaLimit.position, areaLimit.localScale);
        }

        // Desenha a área de ataque quando o objeto está selecionado
        Gizmos.color = Color.blue; // Cor para a área de ataque quando selecionado
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
