using System.Collections;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public GameObject lightningPrefab; // Prefab do raio
    public Transform meleeAttackPoint; // Ponto de ataque melee
    public GameObject areaAttackCollider; // Colisor invisível para o ataque em área
    public float meleeRange = 1.5f; // Alcance do ataque melee
    public int meleeDamage = 10; // Dano do ataque melee
    public float jumpHeight = 5f; // Altura do pulo
    public float moveSpeed = 3f; // Velocidade de movimento
    public float attackCooldown = 3f; // Tempo de espera após cada ataque
    public float dashSpeed = 15f; // Velocidade do dash
    public float dashDuration = 0.3f; // Duração do dash (tempo que o dash irá durar)
    public float dashCooldown = 5f; // Intervalo do dash
    public float lightningRange = 10f; // Alcance do raio de ataque
    public float lightningCooldown = 2f; // Tempo entre os ataques de raio
    public int maxHealth = 100; // Vida máxima do boss
    private int currentHealth; // Vida atual do boss
    public PhaseManager phaseManager; // Referência ao PhaseManager para mudar de fase após a morte do boss

    private Animator anim; // Referência ao Animator
    private Rigidbody2D rb; // Referência ao Rigidbody2D
    private bool isAttacking = false; // Se o boss está atacando
    private bool isWaiting = false; // Se o boss está esperando após atacar
    private bool isDashing = false; // Se o boss está no meio de um dash
    private bool isFacingRight = true; // Direção que o Boss está virado
    private bool isLightningActive = false; // Controle para evitar raios múltiplos
    private bool isHealing = false; // Controle para a cura
    private bool isIdle = true; // Se o boss está em estado de inatividade
    private bool isDead = false; // Se o boss está morto
    private float nextDashTime = 0f; // Tempo para o próximo dash
    private float healingCooldown = 5f; // Tempo de espera para tentar curar

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth; // Inicializa a saúde
        phaseManager = FindObjectOfType<PhaseManager>(); // Busca o PhaseManager para transição de fases

        // Garante que o colisor invisível começa desativado
        if (areaAttackCollider != null)
        {
            areaAttackCollider.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null || isDead) return; // Se o boss estiver morto, não faz mais nada

        // Se o boss está curando, dando dash ou atacando, ele não deve executar outras ações
        if (isHealing || isDashing || isAttacking)
        {
            return; // Impede que qualquer outra ação aconteça
        }

        FollowPlayer();

        // Checar distância para iniciar ataques
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= meleeRange)
        {
            StartCoroutine(PerformComboAttack());
        }
        else if (Time.time >= nextDashTime && distanceToPlayer > meleeRange)
        {
            StartCoroutine(PerformDash());
            nextDashTime = Time.time + dashCooldown;
        }

        // Ataque de raio
        if (distanceToPlayer <= lightningRange && !isAttacking && !isLightningActive)
        {
            StartCoroutine(PerformLightningAttack());
        }

        // Tentar curar com 20% de chance
        if (Random.value <= 0.2f && currentHealth < maxHealth && Time.time >= healingCooldown)
        {
            StartCoroutine(TryHeal());
        }
    }

    private void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // Verifica se o Boss realmente está se movendo
        bool isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;

        // Atualiza o Animator
        anim.SetBool("IsMoving", isMoving);

        // Atualiza a direção
        if (direction.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        // Inverte a direção que o Boss está virado
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private IEnumerator PerformComboAttack()
    {
        isAttacking = true;
        isIdle = false;
        rb.velocity = Vector2.zero; // Para o movimento
        anim.SetBool("IsMoving", false); // Desativa animação de movimento

        // Ativa a animação do ataque melee
        anim.SetTrigger("MeleeCombo");
        yield return new WaitForSeconds(0.2f);
        // Executa o primeiro ataque melee
        PerformMeleeAttack();
        // Aguarda antes do pulo
        yield return new WaitForSeconds(1f);
        // Realiza o pulo
        PerformJump();

        // Aguarda a aterrissagem
        yield return new WaitForSeconds(0.5f);

        // Executa o ataque em área com o colisor invisível
        ActivateAreaAttackCollider();

        // Aguarda 3 segundos após o ataque
        yield return new WaitForSeconds(3f);

        isAttacking = false;
        isIdle = true; // Libera o boss para outra ação
    }

    private void PerformMeleeAttack()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeRange);
        foreach (Collider2D hitPlayer in hitPlayers)
        {
            if (hitPlayer.CompareTag("Player"))
            {
                hitPlayer.GetComponent<PlayerController>()?.TakeDmg(meleeDamage);
            }
        }
    }

    private void PerformJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f); 
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight); 
    }

    private void ActivateAreaAttackCollider()
    {
        if (areaAttackCollider != null)
        {
            areaAttackCollider.SetActive(true);

            // Desativa o colisor após 0.5 segundos
            StartCoroutine(DeactivateAreaAttackCollider());
        }
    }

    private IEnumerator DeactivateAreaAttackCollider()
    {
        yield return new WaitForSeconds(0.5f);
        if (areaAttackCollider != null)
        {
            areaAttackCollider.SetActive(false);
        }
    }

    private IEnumerator PerformLightningAttack()
    {
        isAttacking = true; // Marca como atacando para impedir outras ações
        isLightningActive = true;

        rb.velocity = Vector2.zero; // Para o movimento
        anim.SetBool("IsMoving", false); // Desativa animação de movimento

        // Ativa a animação do ataque de raio
        anim.SetTrigger("BeamAttack");

        // Aguarda o tempo para sincronizar a animação com o ataque
        yield return new WaitForSeconds(0.5f);

        // Instancia o raio
        Vector3 lightningPosition = player.position + Vector3.up * 2f;
        Instantiate(lightningPrefab, lightningPosition, Quaternion.identity);

        // Aguarda 3 segundos após o ataque
        yield return new WaitForSeconds(3f);

        isLightningActive = false;
        isAttacking = false;
    }

    private IEnumerator PerformDash()
    {
        if (isDashing)
        {
            yield break;
        }

        isDashing = true;
        isIdle = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0; 

        Vector2 dashDirection = isFacingRight ? Vector2.right : Vector2.left;
        float dashTime = 0f;

        rb.velocity = new Vector2(dashDirection.x * dashSpeed, rb.velocity.y);

        anim.SetTrigger("Dash");

        while (dashTime < dashDuration)
        {
            dashTime += Time.deltaTime;
            rb.velocity = new Vector2(dashDirection.x * dashSpeed, rb.velocity.y);
            yield return null;
        }

        rb.gravityScale = originalGravity;
        isDashing = false;
        isIdle = true;
    }

    private IEnumerator TryHeal()
    {
        isHealing = true;
        anim.SetTrigger("Heal");

        // Verifica a chance de cura
        if (Random.value <= 0.2f && currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + 20, maxHealth); // Recupera 20 de vida
            healingCooldown = Time.time + 5f; // Define o cooldown de cura
        }

        yield return new WaitForSeconds(1f);
        isHealing = false;
    }

    // Método para o boss receber dano
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public IEnumerator Die()
    {
        isDead = true; 
        anim.SetTrigger("Death");
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(2);
        phaseManager.TriggerNextPhase();
    }
}
