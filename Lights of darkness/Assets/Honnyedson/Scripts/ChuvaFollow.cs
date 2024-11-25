using UnityEngine;

public class RainFollowPlayer : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public float followSpeed = 5f; // Velocidade de movimentação
    public float heightOffset = 5f; // Altura em relação ao jogador
    public Vector3 followOffset = Vector3.zero; // Offset adicional (além da altura)

    private void Update()
    {
        if (player != null)
        {
            // Calcula a posição desejada (acima do jogador)
            Vector3 targetPosition = player.position + followOffset + new Vector3(0, heightOffset, 0);

            // Move suavemente o objeto para a posição desejada
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}