using Unity.VisualScripting;
using UnityEngine;

public class BooEnemy : MonoBehaviour
{
    public GameObject player;
    public float speed = 2f;
    public float stopDistance = 2f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public Transform areaLimit;

    private SpriteRenderer spriteRenderer;       
    private float lastAttackTime;
    private BossController boss;
    public int Life = 3;
    public int dmg;

    // Referências para os objetos de luz
    public GameObject lightObjectLeft;
    public GameObject lightObjectRight;

    // Referência ao Animator
    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boss = FindObjectOfType<BossController>();
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Desativar as luzes inicialmente
        lightObjectLeft.SetActive(false);
        lightObjectRight.SetActive(false);
    }

    void Update()
    {
        if (IsPlayerInArea())
        {
            Vector3 directionToPlayer = player.transform.position - transform.position; 
            float distanceToPlayer = directionToPlayer.magnitude;           
            bool isPlayerLooking = IsPlayerLooking();

            // Atualiza a animação dependendo do estado
            if (Life <= 0)
            {
                animator.SetInteger("Transition", 4); // Morre
                return; // Para a execução se o inimigo estiver morrendo
            }

            if (!isPlayerLooking)
            {
                // Ativar a luz dependendo da direção do inimigo
                if (directionToPlayer.x > 0) // O inimigo está à esquerda do jogador
                {
                    lightObjectRight.SetActive(true);
                    lightObjectLeft.SetActive(false);
                }
                else // O inimigo está à direita do jogador
                {
                    lightObjectLeft.SetActive(true);
                    lightObjectRight.SetActive(false);
                }

                // Muda a camada do inimigo para um grupo de colisão que não recebe dano
                gameObject.layer = LayerMask.NameToLayer("Default");

                // Move o inimigo
                if (distanceToPlayer > stopDistance)
                {
                    MoveTowardsPlayer(directionToPlayer);
                }
                else
                {
                    animator.SetInteger("Transition", 0); // Idle
                }
            }
            else
            {
                // Desativar luz e voltar a camada normal se o jogador está olhando
                lightObjectLeft.SetActive(false);
                lightObjectRight.SetActive(false);
                gameObject.layer = LayerMask.NameToLayer("Enemy"); // Ou outra camada padrão
                animator.SetInteger("Transition", 0); // Idle
            }

            // Lógica de ataque
            if (distanceToPlayer <= attackRange && isPlayerLooking && Time.time > lastAttackTime + attackCooldown)
            {
                animator.SetInteger("Transition", 2); // Atacando
                player.GetComponent<PlayerController>().TakeDmg(dmg);
                lastAttackTime = Time.time; 
            }
        }
        else
        {
            animator.SetInteger("Transition", 0); // Idle se o jogador não estiver na área
        }
    }

    private void MoveTowardsPlayer(Vector3 directionToPlayer)
    {
        Vector3 targetPosition = player.transform.position;

        // Define a altura mínima do fantasma como a altura do jogador
        if (transform.position.y < player.transform.position.y)
        {
            targetPosition.y = player.transform.position.y;
        }

        // Mova o inimigo em direção ao jogador
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        animator.SetInteger("Transition", 1); // Andando

        // Flip do sprite dependendo da direção
        if (directionToPlayer.x < 0)
        {
            spriteRenderer.flipX = true; // Inverter o sprite para a esquerda
        }
        else if (directionToPlayer.x > 0)
        {
            spriteRenderer.flipX = false; // Mostrar o sprite para a direita
        }
    }

    private bool IsPlayerInArea()
    {
        return areaLimit.GetComponent<Collider2D>().bounds.Contains(player.transform.position);
    }

    private bool IsPlayerLooking()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        Vector3 directionToEnemy = (transform.position - player.transform.position).normalized;

        return (playerController.isFacingRight && directionToEnemy.x > 0) ||
               (!playerController.isFacingRight && directionToEnemy.x < 0);
    }

    public void TakeDamage(int damage)
    {
        if (gameObject.layer == LayerMask.NameToLayer("Default")) // Apenas recebe dano na camada padrão
        {
            Life -= damage;
            animator.SetTrigger("Hit");
            if (Life <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log("O inimigo morreu!");
        animator.SetTrigger("Die");
        Destroy(gameObject, 1f); // Aguarda um tempo antes de destruir
    }

    private void OnDrawGizmos()
    {
        if (areaLimit != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(areaLimit.position, areaLimit.localScale);
        }
        
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (areaLimit != null)
        {
            Gizmos.color = Color.yellow; 
            Gizmos.DrawWireCube(areaLimit.position, areaLimit.localScale);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
