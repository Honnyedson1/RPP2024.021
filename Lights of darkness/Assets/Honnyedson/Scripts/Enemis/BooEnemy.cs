using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BooEnemy : MonoBehaviour
{
    public GameObject player;
    public float speed = 2f;
    public float stopDistance = 2f;
    public Vector2 attackSize = new Vector2(1f, 1f); // Tamanho do ataque (largura, altura)
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

    private Coroutine attackCoroutine; // Para gerenciar a coroutine de ataque
    private bool isAttacking = false; // Para verificar se o inimigo está atacando

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
                    lightObjectLeft.SetActive(true);
                }
                else // O inimigo está à direita do jogador
                {
                    lightObjectLeft.SetActive(true);
                    lightObjectRight.SetActive(true);
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

                // Iniciar ataque se dentro do alcance de ataque
                if (IsPlayerInAttackRange() && Time.time > lastAttackTime + attackCooldown)
                {
                    if (attackCoroutine == null) // Se não há ataque em execução
                    {
                        attackCoroutine = StartCoroutine(AttackCoroutine());
                    }
                }
                else if (!isAttacking) // Se não está atacando, muda para idle
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
                // Cancelar ataque se o jogador estiver olhando
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }
            }
        }
        else
        {
            animator.SetInteger("Transition", 0); // Idle se o jogador não estiver na área
            // Cancelar ataque se o jogador não estiver na área
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true; // Define como atacando
        yield return new WaitForSeconds(1.2f); // Espera 0.8 segundos antes de verificar se o jogador ainda está na área
        if (IsPlayerInAttackRange()) // Verifica se o jogador ainda está na área de ataque
        {
            animator.SetTrigger("Attack"); // Ativa o trigger de ataque
            yield return new WaitForSeconds(0.3f);
            player.GetComponent<PlayerController>().TakeDmg(dmg);
            lastAttackTime = Time.time;
        }

        yield return new WaitForSeconds(0.5f); // Espera 0.5 segundos para retornar ao estado Idle
        isAttacking = false; // Define como não atacando
        animator.SetInteger("Transition", 0); // Retorna para Idle

        attackCoroutine = null; // Reseta a referência da coroutine após o ataque
    }

    private void MoveTowardsPlayer(Vector3 directionToPlayer)
    {
        Vector3 targetPosition = player.transform.position;
        targetPosition.y += 1.4f;

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
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (directionToPlayer.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private bool IsPlayerInArea()
    {
        return areaLimit.GetComponent<Collider2D>().bounds.Contains(player.transform.position);
    }

    private bool IsPlayerInAttackRange()
    {
        // Verifica se o jogador está dentro da área do ataque (formato retangular)
        Vector2 attackPosition = (Vector2)transform.position + new Vector2(0, 0.5f); // Ajusta a posição do ataque
        return Physics2D.OverlapBox(attackPosition, attackSize, 0, LayerMask.GetMask("Player")) != null;
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
        // Desenhar a área de ataque como um retângulo
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(0, 0.5f), attackSize);
    }

    private void OnDrawGizmosSelected()
    {
        if (areaLimit != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(areaLimit.position, areaLimit.localScale);
        }
        Gizmos.color = Color.blue;
        // Desenhar a área de ataque como um retângulo
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(0, 0.5f), attackSize);
    }
}
