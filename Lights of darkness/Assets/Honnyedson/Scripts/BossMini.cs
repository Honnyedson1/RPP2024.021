using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossBehavior : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public GameObject healthSlider; // Slider de vida do boss
    public Slider healthBar; // Componente Slider para atualizar a barra de vida
    public float visionRadius = 10f; // Raio de visão do boss
    public GameObject lightningPrefab; // Prefab do raio
    public Transform meleeAttackPoint; // Ponto de ataque melee
    public GameObject areaAttackCollider; // Colisor invisível para o ataque em área
    public float meleeRange = 1.5f; // Alcance do ataque melee
    public int meleeDamage = 1; // Dano do ataque melee
    public float jumpHeight = 5f; // Altura do pulo
    public float moveSpeed = 3f; // Velocidade de movimento
    public float attackCooldown = 3f; // Tempo de espera após cada ataque
    public float dashSpeed = 15f; // Velocidade do dash
    public float dashDuration = 0.3f; // Duração do dash (tempo que o dash irá durar)
    public float lightningRange = 10f; // Alcance do raio de ataque
    public float lightningCooldown = 2f; // Tempo entre os ataques de raio
    public int maxHealth = 100; // Vida máxima do boss
    private int currentHealth; // Vida atual do boss
    private bool isPlayerInSight = false; // Se o jogador está na área de visão
    public PhaseManager phaseManager; // Referência ao PhaseManager para mudar de fase após a morte do boss
    private bool isActionInProgress = false;
    private bool isInPhaseTwo = false; // Marca se o boss está na fase 2
    private float actionCooldown = 3f; // Tempo de espera entre as ações
    private float dashCooldown = 5f; // Cooldown inicial do dash
    private Vector3 initialPosition; // Posição inicial do boss
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
        healthSlider.SetActive(false); // Garante que o slider começa desativado
        currentHealth = maxHealth; // Inicializa a saúde
        healthBar.maxValue = maxHealth; // Configura o valor máximo do slider
        healthBar.value = currentHealth; // Atualiza o slider com a vida atual
        phaseManager = FindObjectOfType<PhaseManager>(); // Busca o PhaseManager para transição de fases

        initialPosition = transform.position; // Salva a posição inicial do boss

        // Garante que o colisor invisível começa desativado
        if (areaAttackCollider != null)
        {
            areaAttackCollider.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null || isDead) return;
        if (GameManager.Instance.Life <= 0)
        {
            ResetBoss();
        }
        if (!isInPhaseTwo && currentHealth <= maxHealth / 2)
        {
            EnterPhaseTwo();
        }

        // Atualiza as ações do boss
        isPlayerInSight = Physics2D.OverlapCircle(transform.position, visionRadius, LayerMask.GetMask("Player")) != null;

        if (!isPlayerInSight)
        {
            healthSlider.SetActive(false);
            return;
        }

        healthSlider.SetActive(true);
        FollowPlayer();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= meleeRange)
        {
            StartCoroutine(PerformActionWithCooldown(PerformComboAttack()));
        }
        else if (Time.time >= nextDashTime && distanceToPlayer > meleeRange)
        {
            StartCoroutine(PerformActionWithCooldown(PerformDash()));
            nextDashTime = Time.time + dashCooldown;
        }

        if (distanceToPlayer <= lightningRange && !isAttacking && !isLightningActive)
        {
            StartCoroutine(PerformActionWithCooldown(PerformLightningAttack()));
        }

        if (Random.value <= 0.2f && currentHealth < maxHealth && Time.time >= healingCooldown)
        {
            StartCoroutine(PerformActionWithCooldown(TryHeal()));
        }
    }

    private void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // Atualiza a direção do boss
        if (direction.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && isFacingRight)
        {
            Flip();
        }

        // Atualiza o Animator
        bool isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;
        anim.SetBool("IsMoving", isMoving);
    }
    private void EnterPhaseTwo()
    {
        isInPhaseTwo = true;
        actionCooldown = 2f; // Reduz o tempo de espera entre as ações
        dashCooldown = 3f; // Reduz o cooldown do dash para 3 segundos
        Debug.Log("Boss entrou na fase 2!");

        // Aqui você pode adicionar animações ou efeitos especiais para a fase 2, se necessário
        anim.SetTrigger("PhaseTwo");
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    private IEnumerator PerformActionWithCooldown(IEnumerator action)
    {
        if (isActionInProgress) yield break;
        isActionInProgress = true;

        // Executa a ação
        yield return StartCoroutine(action);

        // Após a ação, aguarda o cooldown
        yield return new WaitForSeconds(actionCooldown);

        isActionInProgress = false;
    }

    private IEnumerator PerformComboAttack()
    {
        isAttacking = true;
        isIdle = false;
        rb.velocity = Vector2.zero;
        anim.SetBool("IsMoving", false);

        anim.SetTrigger("MeleeCombo");
        yield return new WaitForSeconds(0.2f);
        PerformMeleeAttack(); // Executa o primeiro ataque melee
        yield return new WaitForSeconds(1f);
        PerformJump(); // Executa o pulo
        yield return new WaitForSeconds(0.5f);
        ActivateAreaAttackCollider(); // Executa o ataque em área
        yield return new WaitForSeconds(3f); // Aguarda 3 segundos após a sequência de ataques
        isAttacking = false;
        isIdle = true;
    }

    public void ResetBoss()
    {
        // Reseta o boss para o estado inicial
        transform.position = initialPosition;
        currentHealth = maxHealth;
        healthBar.value = currentHealth; // Atualiza a barra de vida
        healthSlider.SetActive(true); // Ativa o slider de vida
        isDead = false;
        rb.velocity = Vector2.zero;
        anim.SetBool("IsMoving", false);
        anim.ResetTrigger("Death");
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
        isAttacking = true;
        isLightningActive = true;

        rb.velocity = Vector2.zero; // Para o movimento
        anim.SetBool("IsMoving", false); // Desativa animação de movimento

        anim.SetTrigger("BeamAttack");
        yield return new WaitForSeconds(0.5f); // Aguarda o tempo para sincronizar a animação

        Vector3 lightningPosition = player.position + Vector3.up * 2f;
        Instantiate(lightningPrefab, lightningPosition, Quaternion.identity);

        yield return new WaitForSeconds(3f); // Aguarda 3 segundos após o ataque
        isLightningActive = false;
        isAttacking = false;
    }

    private IEnumerator PerformDash()
    {
        if (isDashing) yield break;

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

        yield return new WaitForSeconds(dashCooldown); // Aguarda o cooldown do dash
    }   

    private IEnumerator TryHeal()
    {
        if (currentHealth >= maxHealth) yield break;
        anim.SetTrigger("Heal");

        if (Random.value <= 0.2f)
        {
            currentHealth = Mathf.Min(currentHealth + 20, maxHealth);
            healthBar.value = currentHealth; // Atualiza a barra de vida
        }

        yield return new WaitForSeconds(actionCooldown); // Usa o cooldown atualizado
    }

    // Método para o boss receber dano
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("Hit");
        healthBar.value = currentHealth; // Atualiza a barra de vida no slider

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }
    private void UpdateHealthSlider()
    {
        healthBar.value = currentHealth;
    }

    public IEnumerator Die()
    {
        isDead = true;
        anim.SetTrigger("Death");
        rb.velocity = Vector2.zero;
        healthSlider.SetActive(false); // Desativa o slider após a morte
        yield return new WaitForSeconds(2);

        phaseManager.TriggerNextPhase();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRadius); // Representa a área de visão no Editor
    }
}
