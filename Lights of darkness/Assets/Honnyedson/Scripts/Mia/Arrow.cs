using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage = 1; // Quantidade de dano que a flecha causa

    private void OnTriggerEnter2D(Collider2D other)
    {
        BooEnemy booEnemy = other.GetComponent<BooEnemy>();
        if (booEnemy != null)
        {
            booEnemy.TakeDamage(damage);
            Destroy(gameObject); // Destruir a flecha após causar dano
        }
    }
}