using UnityEngine;
using UnityEngine.Rendering.Universal; // Para acessar Light2D
using UnityEngine.SceneManagement; // Para acessar o SceneManager

public class Arrow : MonoBehaviour
{
    public int damage = 1; // Quantidade de dano que a flecha causa
    public Light2D arrowLight; // Referência ao componente Light2D
    public bool stop; // Flag para indicar se a flecha deve parar
    private Rigidbody2D rb; // Referência ao Rigidbody2D para parar a flecha

    private void Start()
    {
        // Obtém o buildIndex da fase atual
        int currentLevel = SceneManager.GetActiveScene().buildIndex;

        // Verifica se estamos nas fases 3, 4 ou 5 para ativar o Light2D
        if (currentLevel >= 3 && currentLevel <= 5)
        {
            arrowLight.enabled = true; // Ativa a luz
        }
        else
        {
            arrowLight.enabled = false; // Desativa a luz
        }

        // Obtém o Rigidbody2D para manipular o movimento
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se a colisão ocorreu com objetos nas camadas 6 ou 7
        if (other.gameObject.layer == 7 || other.gameObject.layer == 6)
        {
            stop = true;
            rb.velocity = Vector2.zero; // Para o movimento da flecha
            rb.isKinematic = true; // Desativa a física da flecha
        }

        // Verifica se atingiu um BooEnemy
        BooEnemy booEnemy = other.GetComponent<BooEnemy>();
        if (booEnemy != null)
        {
            booEnemy.TakeDamage(damage);
            Destroy(gameObject); // Destruir a flecha após causar dano
        }
        // Verifica se atingiu um BooEnemy
        BossBehavior Mini = other.GetComponent<BossBehavior>();
        if (Mini != null)
        {
            Mini.TakeDamage(damage);
            Destroy(gameObject); // Destruir a flecha após causar dano
        }

        // Verifica se atingiu um DodgeEnemy
        DodgeEnemy dodgeEnemy = other.GetComponent<DodgeEnemy>();
        if (dodgeEnemy != null)
        {
            dodgeEnemy.OnArrowHit();
            Destroy(gameObject);
        }

        // Verifica se atingiu um Enemy genérico
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.takedmg(damage);
            Destroy(gameObject);
        }

        // Verifica se atingiu o Boss
        BossController boss = other.GetComponent<BossController>();
        if (boss != null)
        {
            boss.TakeDamage(damage); // Aplica dano ao Boss
            Destroy(gameObject); // Destrói a flecha após causar dano
        }
    }
}
