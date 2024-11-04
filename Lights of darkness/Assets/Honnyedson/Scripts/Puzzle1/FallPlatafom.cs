using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 0.5f; // Tempo de atraso para a plataforma começar a despencar
    public float respawnTime = 3.0f; // Tempo para a plataforma reaparecer após despencar
    private Vector3 initialPosition; // Posição inicial da plataforma para resetá-la
    private bool isFalling = false; // Verifica se a plataforma já está caindo
    private Rigidbody2D rb; // Referência ao Rigidbody2D

    private void Start()
    {
        initialPosition = transform.position; // Guarda a posição inicial da plataforma
        rb = GetComponent<Rigidbody2D>(); // Obtém o Rigidbody2D da plataforma
        rb.isKinematic = true; // Certifique-se de que a plataforma é cinemática inicialmente
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !isFalling)
        {
            Invoke("Fall", fallDelay); // Inicia o despencar após o tempo de delay
            isFalling = true; // Marca a plataforma como caindo
        }
    }

    private void Fall()
    {
        rb.isKinematic = false; // Permite que a plataforma caia
        Invoke("Respawn", respawnTime); // Agendar o reaparecimento após o tempo especificado
    }

    private void Respawn()
    {
        rb.velocity = Vector2.zero; // Para garantir que a plataforma não esteja em movimento
        rb.isKinematic = true; // Desativa a física temporariamente
        transform.position = initialPosition; // Retorna a plataforma à posição inicial
        isFalling = false; // Define que a plataforma não está mais caindo
    }
}