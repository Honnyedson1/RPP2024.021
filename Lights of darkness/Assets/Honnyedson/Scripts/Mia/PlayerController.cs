using System;
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
    public float attackInterval = 1f; 
    private float lastAttackTime = 0f; 
    
    private static bool isFrozen = false;
    public int damage = 1;


    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isFrozen == false)
        {
                CheckGround();
                Move();
                Jump();
                WallJump();
        
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    ToggleAttackMode();
                }
                
                if (Input.GetKeyDown(KeyCode.Z) && Time.time >= lastAttackTime + attackInterval)
                {
                    StartCoroutine(PerformAttack());
                    lastAttackTime = Time.time;
                }    
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        Vector2 velocity = rb.velocity;

        if (!isWallJumping)
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
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isGrounded = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void WallJump()
    {
        isTouchingWall = Physics2D.OverlapBox(wallCheckPoint.position, wallCheckSize, 0, wallLayer);

        if (isTouchingWall && !isGrounded && Input.GetButtonDown("Jump"))
        {
            isWallJumping = true;
            Vector2 force = new Vector2(wallJumpForceX * -Mathf.Sign(transform.localScale.x), wallJumpForceY);
            rb.velocity = force;
        }

        if (isTouchingWall && !isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -1f));
        }

        if (rb.velocity.y < 0 && !isTouchingWall)
        {
            isWallJumping = false;
        }
    }

    void ToggleAttackMode()
    {
        isMeleeAttack = !isMeleeAttack;
    }

    IEnumerator PerformAttack()
    {
        if (isMeleeAttack)
        {
            MeleeAttack();
        }
        else
        {
            yield return StartCoroutine(RangedAttack());
        }
    }


    void MeleeAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, meleeAttackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            BooEnemy booEnemy = enemy.GetComponent<BooEnemy>();
            DodgeEnemy Enemy2 = enemy.GetComponent<DodgeEnemy>();
            if (booEnemy != null)
            {
                booEnemy.TakeDamage(damage); 
                Debug.Log("Hit enemy with melee attack: " + enemy.name);
            }

            if (Enemy2 != null)
            {
                Enemy2.TakeDamage(damage);
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

    public void Die()
    {
        GameManager.Instance.RespawnPlayer(gameObject);
        GameManager.Instance.Life = 3;
    }

    public static IEnumerator FreezeCoroutine()
    {
        isFrozen = true;
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
    }
}
