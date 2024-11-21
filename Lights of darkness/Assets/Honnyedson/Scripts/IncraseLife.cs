using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public int maxHealthIncrease = 1; // Quantidade de vida máxima adicionada

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.Life += maxHealthIncrease; // Também cura o jogador ao pegar o item
            if (GameManager.Instance.Life > GameManager.Instance.VidaMaxima)
            {
                GameManager.Instance.Life = GameManager.Instance.VidaMaxima; // Evita exceder o máximo
            }
            GameManager.Instance.LifeText.text = $"{GameManager.Instance.Life}/{GameManager.Instance.VidaMaxima}";
            Destroy(gameObject); // Remove o item após ser coletado
        }
    }
}