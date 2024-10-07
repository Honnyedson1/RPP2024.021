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
    private int direction = 1; 

    
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
                if (!isGrounded && rb.velocity.y > 0 && !isJumping)
                {
                    isJumping = true;
                    anim.SetInteger("Transition", 1);
                }
                else if (isGrounded && isJumping)
                {
                    isJumping = false;
                    anim.SetInteger("Transition", 0);
                }
                CheckGround();
                Move();
                WallJump();
                Jump();
                if (Input.GetKeyDown(KeyCode.J))
                {
                    ToggleAttackMode();
                }
                if (Input.GetKeyDown(KeyCode.K) && Time.time >= lastAttackTime + GameManager.Instance.attackInterval)
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
            isWallJumping = false; // Resetar estado de Wall Jump quando tocar no chão
        }
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");

        // Verificar se o jogador está no Wall Jump
        if (!isWallJumping)
        {
            // Permitir movimento apenas se não estiver no Wall Jump
            Vector2 velocity = rb.velocity;
            velocity.x = horizontal * moveSpeed;
            rb.velocity = velocity;
        }

        // Sempre permitir que o jogador vire
        if (horizontal > 0)
        {
            direction = 1; // Jogador virado para a direita
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (horizontal < 0)
        {
            direction = -1; // Jogador virado para a esquerda
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        // Atualizar animação apenas se não estiver atacando, pulando ou deslizando
        if (!isJumping && !isAttacking && !isWallSliding)
        {
            anim.SetInteger("Transition", horizontal != 0 ? 2 : 0);
        }
    }
    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                isJumping = true;
                anim.SetInteger("Transition", 1); 
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (isTouchingWall && !isGrounded)
            {
                isJumping = true;
                isWallJumping = true; // Iniciar o estado de Wall Jump
                anim.SetInteger("Transition", 1);
            
                // Aplicar impulso na direção oposta
                Vector2 force = new Vector2(wallJumpForceX * -direction, wallJumpForceY);
                rb.velocity = force;
            
                StartCoroutine(EndWallJump());
            }
        }
    }


    void WallJump()
    {
        isTouchingWall = Physics2D.OverlapBox(wallCheckPoint.position, wallCheckSize, 0, wallLayer);

        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true; 
            anim.SetInteger("Transition", 3); 
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -2f)); 
        }
        else
        {
            isWallSliding = false; 
        }
    }

    void ToggleAttackMode()
    {
        isMeleeAttack = !isMeleeAttack;
        GameManager.Instance.EstouComArco = !GameManager.Instance.EstouComArco;
    }

    IEnumerator PerformAttack()
    {
        
        rb.velocity = Vector2.zero;
        isAttacking = true;

        if (isMeleeAttack)
        {
            anim.SetInteger("Transition", 5); 
            yield return new WaitForSeconds(0.2f);
            MeleeAttack();
        }
        else
        {
            anim.SetInteger("Transition", 4); 
            yield return new WaitForSeconds(0.2f); 
            yield return StartCoroutine(RangedAttack());
        }

        yield return new WaitForSeconds( GameManager.Instance.attackInterval); 
        isAttacking = false;
    }
    public void SetPlayerControl(bool isEnabled)
    {
        canMove = isEnabled;
    }
    IEnumerator EndWallJump()
    {
        yield return new WaitForSeconds(0.4f); // Tempo de duração do Wall Jump
        isWallJumping = false; // Permitir movimento normal após o Wall Jump
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
                boss.TakeDamage(damage);
            }
        }
    }

    IEnumerator RangedAttack()
    {
        GameObject arrow = Instantiate(arrowPrefab, attackPoint.position, Quaternion.identity);
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

        // Usar a variável direction para disparar a flecha na direção correta
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
