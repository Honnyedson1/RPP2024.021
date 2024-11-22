using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BooEnemy : MonoBehaviour
{
    
    public AudioClip whisperSound; // Som de sussurro
    public AudioClip attackSound;  // Som de ataque
    private AudioSource audioSource;
    
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
    private Vector2 initialPosition;

    // Referência ao Animator
    private Animator animator;
    private Coroutine attackCoroutine; // Para gerenciar a coroutine de ataque
    private bool isAttacking = false; // Para verificar se o inimigo está atacando

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Obter o AudioSource

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boss = FindObjectOfType<BossController>();
        initialPosition = transform.position;

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
    if (GameManager.Instance.Life <= 0)
    {
        transform.position = initialPosition;
    }
    

    if (IsPlayerInArea())
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        bool isPlayerLooking = IsPlayerLooking();

        // Atualiza a animação dependendo do estado
        if (Life <= 0)
        {
            animator.SetInteger("Transition", 4); // Morre
            return;
        }

        if (!isPlayerLooking)
        {
            // Ativar a luz dependendo da direção do inimigo
            if (directionToPlayer.x > 0)
            {
                lightObjectRight.SetActive(true);
                lightObjectLeft.SetActive(true);
            }
            else
            {
                lightObjectLeft.SetActive(true);
                lightObjectRight.SetActive(true);
            }

            // Muda a camada do inimigo para um grupo de colisão que não recebe dano
            gameObject.layer = LayerMask.NameToLayer("Default");

            if (distanceToPlayer > stopDistance)
            {
                MoveTowardsPlayer(directionToPlayer);
                PlayWhisperSound(); // Reproduz o som de sussurro
            }

            else
            {
                animator.SetInteger("Transition", 0); // Idle
            }

            // Iniciar ataque se dentro do alcance de ataque
            if (IsPlayerInAttackRange() && Time.time > lastAttackTime + attackCooldown)
            {
                if (attackCoroutine == null)
                {
                    attackCoroutine = StartCoroutine(AttackCoroutine());
                }
            }
            else if (!isAttacking)
            {
                animator.SetInteger("Transition", 1); // Correndo
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
        StopWhisperSound(); // Para o som de sussurro

    }
}
    private void PlayWhisperSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = whisperSound;
            audioSource.loop = true; // Faz o som de sussurro repetir
            audioSource.Play();
        }
    }

    private void StopWhisperSound()
    {
        if (audioSource.isPlaying && audioSource.clip == whisperSound)
        {
            audioSource.Stop();
        }
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true; 
        animator.SetTrigger("Attack");
        audioSource.PlayOneShot(attackSound);
        yield return new WaitForSeconds(0.2f);
        if (IsPlayerInAttackRange()) 
        {
            player.GetComponent<PlayerController>().TakeDmg(dmg);
            lastAttackTime = Time.time;
        }
        yield return new WaitForSeconds(0.5f); 
        isAttacking = false; 
        animator.SetInteger("Transition", 0);
        attackCoroutine = null; 
    }

    private void MoveTowardsPlayer(Vector3 directionToPlayer)
    {
        Vector3 targetPosition = player.transform.position;
        targetPosition.y += 1.4f;
        
        // Mova o inimigo em direção ao jogador
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        
        // Ativa a animação de movimento enquanto o inimigo se move
        animator.SetInteger("Transition", 1); // Correndo

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
            if (Life <= 0)
            {
                Die();
            }
            else
            {
                animator.SetTrigger("Hit");
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
