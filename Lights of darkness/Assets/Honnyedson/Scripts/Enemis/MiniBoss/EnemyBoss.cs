using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class MiniBoss2D : MonoBehaviour
{
    // Componentes e Referências
    public Animator animator;
    public Transform player;
    public Slider healthSlider;
    public GameObject beamEffect;
    public GameObject groundLightningEffect;

    // Atributos Gerais
    public float maxHealth = 100f;
    private float health;
    public float moveSpeed = 3f;

    // Dash
    public float dashSpeed = 12f;
    public float dashCooldown = 4f;
    private float lastDashTime;
    public float dashDuration = 0.3f;

    // Combate
    public int meleeDamage = 10;
    public int lightningDamage = 15;
    public int fallDamage = 20;
    public float meleeAttackRadius = 1.5f;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    // Alcance
    public float meleeRange = 2f;
    public float beamAttackRange = 5f;
    public float activationRadius = 10f;

    // Checagem de Chão
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    // Estágio 2
    private bool isInSecondStage = false;
    public float secondStageDashCooldown = 2f;
    public float secondStageDamageMultiplier = 1.5f;

    // Estados
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
            healthSlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDead) return;

        UpdateAnimatorParameters();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Checagem de chão
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Controle do slider de vida
        if (healthSlider != null)
            healthSlider.gameObject.SetActive(distanceToPlayer <= activationRadius);

        // Muda para o segundo estágio se necessário
        if (health <= maxHealth / 2 && !isInSecondStage)
        {
            EnterSecondStage();
        }

        // Comportamento do boss
        if (!isAttacking)
        {
            if (distanceToPlayer <= meleeRange)
                StartCoroutine(MeleeAttack());
            else if (distanceToPlayer <= beamAttackRange)
                StartCoroutine(BeamAttack());
            else if (distanceToPlayer <= activationRadius && isGrounded)
                MoveTowardsPlayer();
            else
                StopMoving();

            // Dash aleatório para se aproximar
            if (Time.time >= lastDashTime + dashCooldown && distanceToPlayer > meleeRange)
                StartCoroutine(PerformDash());
        }
    }

    private void UpdateAnimatorParameters()
    {
        // Atualiza o estado de "isRunning" com base no movimento horizontal
        bool isRunning = Mathf.Abs(rb.velocity.x) > 0.1f;
        animator.SetBool("isRunning", isRunning);
    }

    private void EnterSecondStage()
    {
        isInSecondStage = true;
        dashCooldown = secondStageDashCooldown;
        meleeDamage = Mathf.RoundToInt(meleeDamage * secondStageDamageMultiplier);
        lightningDamage = Mathf.RoundToInt(lightningDamage * secondStageDamageMultiplier);
        animator.SetTrigger("SecondStage");
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // Ajusta a direção do sprite
        transform.eulerAngles = direction.x > 0 ? Vector3.zero : new Vector3(0, 180, 0);
    }

    private void StopMoving()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private IEnumerator PerformDash()
    {
        isAttacking = true;
        animator.SetTrigger("Dash");
        lastDashTime = Time.time;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector2.zero;
        isAttacking = false;
    }

    private IEnumerator MeleeAttack()
    {
        isAttacking = true;
        animator.SetTrigger("MeleeAttack");
        yield return new WaitForSeconds(0.5f); // Delay para o ataque

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, meleeAttackRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                ApplyDamageToPlayer(meleeDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private IEnumerator BeamAttack()
    {
        isAttacking = true;
        animator.SetTrigger("BeamAttack");

        Vector3 beamPosition = player.position + Vector3.up * 1.5f;
        GameObject beam = Instantiate(beamEffect, beamPosition, Quaternion.identity);

        if (beam.TryGetComponent(out BeamEffect beamScript))
            beamScript.damage = lightningDamage;

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    private void ApplyDamageToPlayer(int damage)
    {
        var playerHealth = player.GetComponent<PlayerController>();
        if (playerHealth != null)
            playerHealth.TakeDmg(damage);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        if (healthSlider != null) healthSlider.value = health;

        if (health <= 0)
            Die();
        else
            animator.SetTrigger("Hit");
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        rb.velocity = Vector2.zero;

        if (healthSlider != null)
            healthSlider.gameObject.SetActive(false);

        // Pode adicionar lógica de destruição ou loot
    }
}
