using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 0.5f; // Tempo de atraso para a plataforma começar a despencar
    public float respawnTime = 3.0f; // Tempo para a plataforma reaparecer após despencar
    private Vector3 initialPosition; // Posição inicial da plataforma para resetá-la
    private bool isFalling = false; // Verifica se a plataforma já está caindo
    private Rigidbody2D rb; // Referência ao Rigidbody2D
    private Collider2D platformCollider; // Referência ao Collider2D
    private bool playerOnPlatform = false; // Verifica se o jogador está na plataforma

    private void Start()
    {
        initialPosition = transform.position; // Guarda a posição inicial da plataforma
        rb = GetComponent<Rigidbody2D>(); // Obtém o Rigidbody2D da plataforma
        platformCollider = GetComponent<Collider2D>(); // Obtém o Collider2D da plataforma
        rb.isKinematic = true; // Certifique-se de que a plataforma é cinemática inicialmente
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !isFalling)
        {
            // Obtém o script do jogador para verificar o estado do groundCheck
            var player = collision.collider.GetComponent<PlayerController>(); // Substitua pelo nome correto do script
            if (player != null && player.isGrounded)
            {
                playerOnPlatform = true; // Marca que o jogador está na plataforma
                Invoke("Fall", fallDelay); // Inicia o despencar após o tempo de delay
                isFalling = true; // Marca a plataforma como caindo
            }
        }
        else if (isFalling && (collision.gameObject.layer == 6 || collision.gameObject.layer == 7))
        {
            // Camada 6 e 7 são exemplo de chão/parede — ajuste conforme necessário
            DisableCollider();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOnPlatform = false; // Marca que o jogador saiu da plataforma

            if (isFalling) // Se estiver caindo e o jogador sair
            {
                DisableCollider();
            }
        }
    }

    private void Fall()
    {
        rb.isKinematic = false; // Permite que a plataforma caia
        if (!playerOnPlatform)
        {
            DisableCollider(); // Desativa o colisor se o jogador não estiver na plataforma
        }
        Invoke("Respawn", respawnTime); // Agendar o reaparecimento após o tempo especificado
    }

    private void Respawn()
    {
        rb.velocity = Vector2.zero; // Para garantir que a plataforma não esteja em movimento
        rb.isKinematic = true; // Desativa a física temporariamente
        transform.position = initialPosition; // Retorna a plataforma à posição inicial
        platformCollider.enabled = true; // Reativa o colisor da plataforma
        isFalling = false; // Define que a plataforma não está mais caindo
    }

    private void DisableCollider()
    {
        platformCollider.enabled = false; // Desativa o colisor
    }
}
