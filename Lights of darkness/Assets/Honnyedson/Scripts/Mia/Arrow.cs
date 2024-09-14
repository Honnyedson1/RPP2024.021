using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage = 1; // Quantidade de dano que a flecha causa

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se atingiu um BooEnemy
        BooEnemy booEnemy = other.GetComponent<BooEnemy>();
        if (booEnemy != null)
        {
            booEnemy.TakeDamage(damage);
            Destroy(gameObject); // Destruir a flecha após causar dano
        }

        // Verifica se atingiu um DodgeEnemy
        DodgeEnemy dodgeEnemy = other.GetComponent<DodgeEnemy>();
        if (dodgeEnemy != null)
        {
            dodgeEnemy.OnArrowHit();
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
