using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController _instance;

    [Header("Wall Jump And Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float wallJumpForceX = 8f;
    public float wallJumpForceY = 10f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isWallJumping;
    private bool isTouchingWall;
    public Transform wallCheckPoint;
    public Vector2 wallCheckSize;
    public LayerMask wallLayer;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isWallSliding = false;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isAttacking = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Attack Settings")]
    public float meleeAttackRange = 1f;
    public float rangedAttackRange = 5f;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public GameObject arrowPrefab; 
    public float arrowSpeed = 10f;
    private bool isMeleeAttack = true;
    private float lastAttackTime = 0f; 
    
    private static bool isFrozen = false;
    public bool canMove = false;
    public int damage = 1;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (canMove)
        {
            if (!isFrozen)
            {
                CheckGround();
                Move();
                WallJump();
                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                }
        
                HandleAnimations(); // Gerencia as animações
        
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    ToggleAttackMode();
                }
        
                if (Input.GetKeyDown(KeyCode.Z) && Time.time >= lastAttackTime + GameManager.Instance.attackInterval)
                {
                    StartCoroutine(PerformAttack());
                    lastAttackTime = Time.time;
                }
            }    
        }

    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            isJumping = false;
            isWallSliding = false;
        }
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        Vector2 velocity = rb.velocity;

        if (!isWallJumping && !isJumping)
        {
            velocity.x = horizontal * moveSpeed;
            rb.velocity = velocity;

            if (horizontal != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(horizontal), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                // Pulo normal
                isJumping = true;
                anim.SetInteger("Transition", 1); // Ativa a animação de pulo
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (isTouchingWall && !isGrounded)
            {
                // Wall Jump
                isJumping = true;
                anim.SetInteger("Transition", 1); // Ativa a animação de pulo
                Vector2 force = new Vector2(wallJumpForceX * -Mathf.Sign(transform.localScale.x), wallJumpForceY);
                rb.velocity = force;

                // Inicia o Wall Slide e o encerra após um breve delay
                StartCoroutine(EndWallSlide());
            }
        }
    }

    void WallJump()
    {
        isTouchingWall = Physics2D.OverlapBox(wallCheckPoint.position, wallCheckSize, 0, wallLayer);

        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true; // Ativa o Wall Slide
            anim.SetInteger("Transition", 3); // Ativa a animação de Wall Slide
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -2f)); // Controla a velocidade de deslizamento
        }
        else
        {
            isWallSliding = false; // Desativa o Wall Slide quando não estiver tocando na parede
        }
    }

    IEnumerator EndWallSlide()
    {
        yield return new WaitForSeconds(0.3f); // Ajuste conforme necessário
        isWallSliding = false;
        isJumping = false;
    }

    void ToggleAttackMode()
    {
        isMeleeAttack = !isMeleeAttack;
    }

    IEnumerator PerformAttack()
    {
        // Para o jogador por 0.2 segundos durante o ataque
        rb.velocity = Vector2.zero;
        isAttacking = true;

        if (isMeleeAttack)
        {
            anim.SetInteger("Transition", 5); // Ativa a animação de ataque com espada
            yield return new WaitForSeconds(0.2f); // Tempo da animação de ataque
            MeleeAttack();
        }
        else
        {
            anim.SetInteger("Transition", 4); // Ativa a animação de ataque com arco
            yield return new WaitForSeconds(0.2f); // Tempo da animação de ataque
            yield return StartCoroutine(RangedAttack());
        }

        yield return new WaitForSeconds( GameManager.Instance.attackInterval); // Espera o intervalo do ataque
        isAttacking = false;
    }
    public void SetPlayerControl(bool isEnabled)
    {
        canMove = isEnabled;
    }

    void HandleAnimations()
    {
        // Idle Animation
        if (isGrounded && Mathf.Abs(rb.velocity.x) < 0.1f && !isAttacking && !isJumping && !isWallSliding)
        {
            anim.SetInteger("Transition", 0); // Idle
        }
        // Walk Animation
        else if (isGrounded && Mathf.Abs(rb.velocity.x) > 0.1f && !isJumping && !isWallSliding)
        {
            anim.SetInteger("Transition", 2); // Walk
        }
        // Wall Slide Animation
        else if (isWallSliding)
        {
            anim.SetInteger("Transition", 3); // Wall Slide
        }
        // Jump Animation
        else if (isJumping && !isGrounded)
        {
            anim.SetInteger("Transition", 1); // Jump
        }
        // Melee Attack Animation
        else if (isAttacking && isMeleeAttack)
        {
            anim.SetInteger("Transition", 5); // Attack Sword
        }
        // Ranged Attack Animation
        else if (isAttacking && !isMeleeAttack)
        {
            anim.SetInteger("Transition", 4); // Attack Bow
        }
    }

    void MeleeAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, meleeAttackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            BooEnemy booEnemy = enemy.GetComponent<BooEnemy>();
            DodgeEnemy enemy2 = enemy.GetComponent<DodgeEnemy>();
            if (booEnemy != null)
            {
                booEnemy.TakeDamage(damage);
            }

            if (enemy2 != null)
            {
                enemy2.TakeDamage(damage);
            }
            BossController boss = enemy.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage); // Causa dano no Boss
            }
        }
    }

    IEnumerator RangedAttack()
    {
        GameObject arrow = Instantiate(arrowPrefab, attackPoint.position, Quaternion.identity);
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        float direction = Mathf.Sign(transform.localScale.x);
        arrowRb.velocity = new Vector2(direction * arrowSpeed, 0);
        Destroy(arrow, 2f);

        yield return null;
    }

    public void TakeDmg(int dmg)
    {
        GameManager.Instance.Life -= dmg;

        if (GameManager.Instance.Life <= 0)
        {
            StartCoroutine(DieAndRespawn());
        }
    }

    private IEnumerator DieAndRespawn()
    {
        isFrozen = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1);
        GameManager.Instance.RespawnPlayer();
        isFrozen = false;
    }

    public IEnumerator FreezeCoroutine()
    {
        isFrozen = true;
        GameManager.Instance.Life--;
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(3);
        isFrozen = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPoint.position, wallCheckSize);

        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, meleeAttackRange);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "EspcialProject")
        {
            StartCoroutine(FreezeCoroutine());
        }
        if (col.gameObject.tag == "BossAtack")
        {
            GameManager.Instance.Life--;
            Destroy(col.gameObject, 0.2f);
        }
    }
}
