using UnityEngine;

public class BeamEffect : MonoBehaviour
{
    public int damage = 3; // Dano padrão do raio
    public float duration = 1.5f; // Duração do raio antes de ser destruído

    private void Start()
    {
        // Destroi o raio automaticamente após a duração
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto colidido tem a tag "Player"
        if (collision.CompareTag("Player"))
        {
            var playerHealth = collision.GetComponent<PlayerController>();
            if (playerHealth != null)
            {
                playerHealth.TakeDmg(damage);
            }
        }
    }
}