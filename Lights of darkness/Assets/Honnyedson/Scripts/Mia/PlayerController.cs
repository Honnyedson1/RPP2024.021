using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IObserver<Coin>
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

    [Header("Observer Manager")]
    private CoinManager coinManager;
    [SerializeField] private int coinCount = 0;

    [Header("Variaveis")]
    public int health;

    public Text Coin;

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
        coinManager = FindObjectOfType<CoinManager>();
        coinManager.Attach(this);
    }

    void Update()
    {
        CheckGround();
        Move();
        Jump();
        WallJump();
        Coin.text = coinCount.ToString();
        if (health <= 0)
        {
            Die();
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
    private void OnDestroy()
    {
        coinManager.Detach(this);
    }
    private void Die()
    {
        GameManager.Instance.RespawnPlayer(gameObject);
        health = 3; 
    }

    public void OnNext(Coin value)
    {
        coinCount++;
        GameManager.Instance.AddScore(1);
    }

    public void OnError(System.Exception error)
    {
        Debug.LogError($"Erro ao coletar moeda: {error.Message}");
    }

    public void OnCompleted()
    {
        Debug.Log("Todas as moedas foram coletadas.");
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
    }
}
