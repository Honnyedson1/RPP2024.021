using System.Collections;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public GameObject rangeAttackPrefab; // Prefab do ataque à distância
    public GameObject meleeImpactEffect; // Efeito do último golpe do melee
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float dashCooldown = 3f;
    public int meleeDamage = 10;
    public float healAmount = 15f;
    public float maxHealth = 100f;
    public float currentHealth;

    public float groundCheckDistance = 1f; // Distância do Raycast para checar o chão
    public LayerMask groundLayer; // Camada do chão
    public Collider2D meleeRange; // Colisor para o alcance do melee
    public Collider2D rangeAttackRange; // Colisor para o alcance do ataque à distância

    private bool isDashing = false;
    private bool isInSecondStage = false;
    private Rigidbody2D rb;
    private Coroutine meleeAttackCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (meleeImpactEffect != null)
        {
            meleeImpactEffect.SetActive(false); // Garante que o objeto de impacto comece desativado
        }
    }

    void Update()
    {
        if (IsOnGround())
        {
            MoveTowardsPlayer();
        }

        if (currentHealth <= maxHealth / 2 && !isInSecondStage)
        {
            EnterSecondStage();
        }

        if (!isDashing && Random.Range(0f, 1f) < 0.01f)
        {
            Dash();
        }
    }

    // Checa se o inimigo está no chão
    private bool IsOnGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    // Movimento em direção ao jogador
    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    // Realiza o ataque corpo a corpo
    public void MeleeAttack()
    {
        if (meleeAttackCoroutine != null)
        {
            StopCoroutine(meleeAttackCoroutine);
        }
        meleeAttackCoroutine = StartCoroutine(ExecuteMeleeAttack());
    }

    private IEnumerator ExecuteMeleeAttack()
    {
        for (int i = 0; i < 3; i++)
        {
            if (meleeRange.IsTouchingLayers(LayerMask.GetMask("Player"))) // Verifica se o jogador está no alcance melee
            {
                player.GetComponent<PlayerController>().TakeDmg(meleeDamage);
            }

            if (i == 2 && meleeImpactEffect != null)
            {
                meleeImpactEffect.SetActive(true); // Ativa o impacto no último golpe
            }

            yield return new WaitForSeconds(0.5f);
        }

        if (meleeImpactEffect != null)
        {
            yield return new WaitForSeconds(1f);
            meleeImpactEffect.SetActive(false);
        }
    }

    // Realiza o ataque à distância
    public void RangeAttack()
    {
        if (rangeAttackRange.IsTouchingLayers(LayerMask.GetMask("Player"))) // Verifica se o jogador está no alcance do ataque à distância
        {
            Vector3 spawnPosition = player.position + Vector3.up * 2f; // Ajuste a altura conforme necessário
            Instantiate(rangeAttackPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // Realiza o dash em direção ao jogador
    private void Dash()
    {
        isDashing = true;
        Vector2 dashDirection = (player.position - transform.position).normalized;
        rb.velocity = dashDirection * dashSpeed;

        StartCoroutine(DashCooldown());
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        isDashing = false;
    }

    // Cura o inimigo
    public void Heal()
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
    }

    private void EnterSecondStage()
    {
        isInSecondStage = true;
        dashCooldown = 1.5f;
        meleeDamage *= 1;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
