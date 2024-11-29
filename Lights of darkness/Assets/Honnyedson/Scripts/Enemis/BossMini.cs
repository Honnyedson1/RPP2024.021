using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossBehavior : MonoBehaviour
{
    public Slider healthSlider; // Referência ao slider de vida

    public float actionCooldown = 1f; // Cooldown para esperar entre ações
    private float lastActionTime = 0f; // Marca o tempo da última ação realizada
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
    public int currentHealth; // Vida atual do boss
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

    public Collider2D attackAreaCollider; // Colisor invisível para a área de ataque
    private bool isInAttackArea = false; // Flag para verificar se o jogador está na área de ataque
    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth; // Inicializa a saúde
        phaseManager = FindObjectOfType<PhaseManager>(); // Busca o PhaseManager para transição de fases

        // Inicializa o slider de vida
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Garante que o colisor invisível começa desativado
        if (areaAttackCollider != null)
        {
            areaAttackCollider.SetActive(false);
        }
    }

    private void Update()
    {
        phaseManager = FindObjectOfType<PhaseManager>(); // Busca o PhaseManager para transição de fases
        if (player == null || isDead) return; // Se o boss estiver morto, não faz mais nada

        // Verifica a distância do jogador
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (healthSlider != null)
        {
            // Oculta o slider de vida se o jogador estiver fora do alcance
            healthSlider.gameObject.SetActive(distanceToPlayer <= 15f);
        }

        // Verifica se já passou o tempo suficiente desde a última ação
        if (Time.time < lastActionTime + actionCooldown) return;

        // Se o boss estiver executando uma ação, não faz mais nada
        if (isHealing || isDashing || isAttacking)
        {
            return; // Impede que outra ação seja iniciada enquanto o boss está ocupado
        }

        // Só permite que o Boss execute ações de ataque se a vida do jogador for maior que zero
        if (GameManager.Instance.Life > 0)
        {
            FollowPlayer();

            // Checar distância para iniciar ataques
            if (distanceToPlayer <= meleeRange && !isHealing && !isDashing && !isAttacking)
            {
                StartCoroutine(PerformComboAttack());
            }
            else if (Time.time >= nextDashTime && distanceToPlayer > meleeRange && !isHealing && !isAttacking)
            {
                StartCoroutine(PerformDash());
                nextDashTime = Time.time + dashCooldown;
            }

            if (distanceToPlayer <= lightningRange && !isAttacking && !isHealing && !isDashing && !isLightningActive)
            {
                StartCoroutine(PerformLightningAttack());
            }

            if (Random.value <= 0.2f && currentHealth < maxHealth && Time.time >= healingCooldown && !isAttacking && !isDashing)
            {
                StartCoroutine(TryHeal());
            }
        }
    }

// Coroutine para resetar o estado do boss após 3 segundos de o jogador morrer
private IEnumerator ResetBossAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);

    // Reseta o estado do boss
    ResetBossState();
}

// Reseta o estado do Boss
    private void ResetBossState()
    {
        // Só reseta o estado do Boss se o jogador estiver vivo
        if (GameManager.Instance.Life > 0) // Verifica se o jogador está vivo
        {
            // Reseta as variáveis importantes
            currentHealth = maxHealth; // Reseta a vida para o valor máximo
            isDead = false; // O boss não está mais morto
            isAttacking = false; // O boss não está mais atacando
            isWaiting = false; // O boss não está esperando
            isDashing = false; // O boss não está mais em dash
            isFacingRight = true; // O boss está virado para a direita inicialmente
            isLightningActive = false; // O boss não está mais com o raio ativo
            isHealing = false; // O boss não está mais curando
            isIdle = true; // O boss está em inatividade
            nextDashTime = Time.time + dashCooldown; // Garante que o dash só poderá ocorrer após o cooldown
            healingCooldown = Time.time + 5f; // Reseta o cooldown da cura

            // Atualiza o slider de vida
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }

            // Coloca o boss de volta à posição inicial ou qualquer outro reset necessário (opcional)
            transform.position = new Vector2(-442.117f, -9.927f); // Por exemplo, resetando a posição do boss
        }
        else
        {
            // Caso o jogador tenha morrido, reseta apenas as variáveis sem mover o boss
            isDead = true; // Garantir que o Boss está em estado de morte
            isAttacking = false;
            isDashing = false;
            isIdle = true;

            // Evita que o Boss continue se movendo ou atacando
            rb.velocity = Vector2.zero; // Garante que o Boss pare de se mover
            anim.SetBool("IsMoving", false); // Para a animação de movimento
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
        yield return new WaitForSeconds(0.2f); // Tempo de delay para a animação
        PerformMeleeAttack(); // Executa o ataque melee

        yield return new WaitForSeconds(1f); // Aguarda antes do pulo
        PerformJump(); // Realiza o pulo

        yield return new WaitForSeconds(0.5f); // Aguarda a aterrissagem
        ActivateAreaAttackCollider(); // Executa o ataque de área

        yield return new WaitForSeconds(3f); // Aguarda 3 segundos após o ataque

        isAttacking = false;
        isIdle = true;

        // Atualiza o tempo da última ação para o cooldown
        lastActionTime = Time.time;
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
            yield break; // Se o boss já estiver dashing, evita o início de outro dash
        }

        isDashing = true;
        isIdle = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0; // Desativa a gravidade temporariamente

        Vector2 dashDirection = isFacingRight ? Vector2.right : Vector2.left;
        float dashTime = 0f;

        rb.velocity = new Vector2(dashDirection.x * dashSpeed, rb.velocity.y);

        anim.SetTrigger("Dash");

        while (dashTime < dashDuration)
        {
            dashTime += Time.deltaTime;
            rb.velocity = new Vector2(dashDirection.x * dashSpeed, rb.velocity.y); // Realiza o dash
            yield return null;
        }

        rb.gravityScale = originalGravity;
        isDashing = false;
        isIdle = true;

        // Atualiza o tempo da última ação para o cooldown
        lastActionTime = Time.time;
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

            // Atualiza o slider de vida após a cura
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }
        }

        yield return new WaitForSeconds(1f);
        isHealing = false;

        // Atualiza o tempo da última ação para o cooldown
        lastActionTime = Time.time;
    }
    // Método para o boss receber dano
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (isAttacking == false)
        {
            anim.SetTrigger("Hit");
        }

        // Atualiza o slider de vida
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

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