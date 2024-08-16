using UnityEngine;

public class PlayerWallJump : MonoBehaviour
{
    [Header("Wall Slide and Jump")]
    public float wallSlideSpeed = 0.5f; // Velocidade de deslize ao tocar a parede
    public float wallJumpForce = 20f;   // Força do pulo na parede
    public Vector2 wallJumpDirection = new Vector2(1, 2); // Direção do pulo

    [Header("Ground Check")]
    public Transform groundCheck; // Ponto de verificação para contato com o chão
    public float groundCheckRadius = 0.2f; // Raio da verificação de contato com o chão
    public LayerMask groundLayer; // Camada do chão

    [Header("Wall Check")]
    public Transform wallCheck; // Ponto de verificação para contato com a parede
    public float wallCheckDistance = 0.2f; // Distância do raycast para verificação da parede
    public LayerMask wallLayer; // Camada das paredes

    private Rigidbody2D rb;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isGrounded;
    private bool isFacingRight = true; // Indica se o jogador está virado para a direita

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Verifica se o jogador está no chão
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Determina se o jogador deve deslizar na parede
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        // Aplicar velocidade de deslize quando tocando a parede
        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        
    }

    private void OnDrawGizmos()
    {
        // Desenhar gizmos para o ground check
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // Desenhar gizmos para o wall check
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (isFacingRight ? Vector3.right : Vector3.left) * wallCheckDistance);
    }
}
