using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController _instance;
    public bool isFacingRight = true;
    [Header("Wall Jump And Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float wallJumpForceX = 8f;
    public float wallJumpForceY = 10f;
    public Rigidbody2D rb;
    public bool isGrounded;
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

    [Header("Double Jump Settings && Dash")]
    public bool doubleJumpUnlocked = false; // Para verificar se o jogador desbloqueou o pulo duplo
    private bool canDoubleJump = false;
    public bool dashUnlocked = false;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private float dashCooldown = 1f;
    private float lastDashTime;// Direção do dash (definida pelo jogador)

    
    private static bool isFrozen = false;
    public bool canMove = false;
    public int damage = 1;

    public Animator anim;

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
                Dash();
                CheckGround(); 
                WallJump(); 
                Move();
                Jump();
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
            isWallJumping = false;
            canDoubleJump = GameManager.Instance.hasDoubleJump; // Permite o pulo duplo quando o jogador está no chão
        }
    }

    void Dash()
    {
        if (GameManager.Instance.hasDash == true && Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + GameManager.Instance.TimeToNextDesh && !isDashing)
        {
            StartCoroutine(PerformDash());
            lastDashTime = Time.time;
        }
    }


    IEnumerator PerformDash()
    {
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0; // Desativa temporariamente a gravidade para um dash horizontal puro

        // Define a velocidade do dash diretamente, em linha reta na direção em que o jogador está virado
        rb.velocity = new Vector2(isFacingRight ? dashSpeed : -dashSpeed, 0);
        anim.SetInteger("Transition", 7); // Animação de dash

        // Impede movimento até que o dash termine
        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity; // Restaura a gravidade ao final do dash
        isDashing = false;
        anim.SetInteger("Transition", 0); // Retorna à animação padrão
    }

    void Move()
    {
        if (isDashing) return; // Evita interferência no dash

        float horizontal = Input.GetAxis("Horizontal");
        if (!isWallJumping)
        {
            Vector2 velocity = rb.velocity;
            velocity.x = horizontal * moveSpeed;
            rb.velocity = velocity;
        }
        if (horizontal > 0)
        {
            isFacingRight = true;
            direction = 1; 
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (horizontal < 0)
        {
            isFacingRight = false;
            direction = -1; 
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
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
                canDoubleJump = GameManager.Instance.hasDoubleJump; // Permite o pulo duplo se desbloqueado
                anim.SetInteger("Transition", 1); 
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (isTouchingWall && !isGrounded)
            {
                if (!isWallJumping)
                {
                    isJumping = true;
                    isWallJumping = true;
                    anim.SetInteger("Transition", 1);
                    Vector2 force = new Vector2(wallJumpForceX * -direction, wallJumpForceY);
                    rb.velocity = force;
                    StartCoroutine(EndWallJump());
                }
            }
            else if (canDoubleJump && isTouchingWall == false)
            {
                canDoubleJump = false; // Desativa o pulo duplo após o uso
                anim.SetInteger("Transition", 1);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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
        GameManager.Instance.EstouComArco = !GameManager.Instance.EstouComArco;
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        if (GameManager.Instance.EstouComArco == false && isWallSliding == false)
        {
            if (isGrounded == true)
            {
                rb.velocity = Vector2.zero;
                canMove = false;
                anim.SetInteger("Transition", 5); 
                yield return new WaitForSeconds(0.5f);
                MeleeAttack();
            }
            else
            {
                anim.SetInteger("Transition", 5); 
                yield return new WaitForSeconds(0.4f);
                MeleeAttack();
            }
        }
        else
        {
            if (GameManager.Instance.QFlechas > 0 && isWallSliding == false)
            {
                if (isGrounded == true)
                {
                    rb.velocity = Vector2.zero;
                    canMove = false;
                    anim.SetInteger("Transition", 4); 
                    yield return new WaitForSeconds(0.2f); 
                    yield return StartCoroutine(RangedAttack());
                    yield return new WaitForSeconds(0.2f);     
                }
                else
                {
                    anim.SetInteger("Transition", 4); 
                    yield return new WaitForSeconds(0.2f); 
                    yield return StartCoroutine(RangedAttack());
                    yield return new WaitForSeconds(0.2f);  
                }

            }

        }
        canMove = true;
        isAttacking = false;
        yield return new WaitForSeconds( GameManager.Instance.attackInterval);
    }
    public void SetPlayerControl(bool isEnabled)
    {
        canMove = isEnabled;
        isFrozen = !isEnabled;
    }

    IEnumerator EndWallJump()
    {
        yield return new WaitForSeconds(0.5f);
        isWallJumping = false;
        canMove = true; 
        isFacingRight = !isFacingRight;
        direction = isFacingRight ? 1 : -1;
        transform.eulerAngles = isFacingRight ? Vector3.zero : new Vector3(0, 180, 0);
    }


    void MeleeAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, meleeAttackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            BooEnemy booEnemy = enemy.GetComponent<BooEnemy>();
            DodgeEnemy enemy2 = enemy.GetComponent<DodgeEnemy>();
            BossController boss = enemy.GetComponent<BossController>();
            Enemy EnemySpawner = enemy.GetComponent<Enemy>();
            BossBehavior Mini = enemy.GetComponent<BossBehavior>();
            if (booEnemy != null)
            {
                booEnemy.TakeDamage(damage);
            }
            if (enemy2 != null)
            {
                enemy2.TakeDamage(damage);
            }
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }

            if (EnemySpawner!=null)
            {
                EnemySpawner.takedmg(damage);
            }

            if (Mini != null)
            {
                Mini.TakeDamage(damage);
            }
        }
    }

    IEnumerator RangedAttack()
    {
        // Instancia a flecha na posição do ponto de ataque
        GameObject arrow = Instantiate(arrowPrefab, attackPoint.position, Quaternion.identity);
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        GameManager.Instance.QFlechas--;

        // Define a direção da flecha com base no valor de "direction"
        arrowRb.velocity = new Vector2(direction * arrowSpeed, 0);

        // Ajusta a rotação da flecha para que ela aponte para a direção em que está sendo disparada
        if (direction < 0)
        {
            arrow.transform.rotation = Quaternion.Euler(0, 0, 180);
        }

        // Destroi a flecha após 2 segundos
        Destroy(arrow, 2f);
        yield return null;
    }

    public void TakeDmg(int dmg)
    {
        if (GameManager.Instance.Life > 0) // Garante que só aplicamos dano se ainda houver vida
        {
            GameManager.Instance.Life -= dmg;
            anim.SetTrigger("Hit"); // Ativa a animação de hit

            if (GameManager.Instance.Life <= 0)
            {
                StartCoroutine(DieAndRespawn());
            }
        }
    }

    public void MorteInstantanea()
    {
        TakeDmg(10);
    }

    public IEnumerator DieAndRespawn()
    {
        isFrozen = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Die"); // Ativa a animação de morte

        yield return new WaitForSeconds(1); // Tempo para a animação de morte

        GameManager.Instance.ShowRespawnPanel(); // Mostra o painel de respawn
    }
    public IEnumerator FreezeCoroutine()
    {
        isFrozen = true;
        TakeDmg(1);
        anim.SetTrigger("Freeze");
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(2);
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
            TakeDmg(1);
            Destroy(col.gameObject, 0.2f);
        }
    }
}