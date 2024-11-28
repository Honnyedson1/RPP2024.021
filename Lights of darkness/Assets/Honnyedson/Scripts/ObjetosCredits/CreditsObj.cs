using UnityEngine;

public class CreditTrigger2D : MonoBehaviour
{
    public CreditTriggerManager2D creditManager; // Referência ao gerenciador

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Certifique-se de que o jogador tem a tag "Player"
        {
            Debug.Log($"Trigger {name} ativado pelo objeto {other.name}.");
            creditManager.TriggerActivated(gameObject); // Comunica ao gerenciador
        }
    }
}