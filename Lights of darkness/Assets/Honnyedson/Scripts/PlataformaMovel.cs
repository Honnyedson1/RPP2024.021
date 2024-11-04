using UnityEngine;

public class PlataformaMovel : MonoBehaviour
{
    public Vector3 pontoA; // Ponto inicial
    public Vector3 pontoB; // Ponto final
    public float velocidade = 2f; // Velocidade de movimento

    private Vector3 proximoPonto;

    private void Start()
    {
        proximoPonto = pontoB;
    }

    private void Update()
    {
        // Move a plataforma para o próximo ponto
        transform.position = Vector3.MoveTowards(transform.position, proximoPonto, velocidade * Time.deltaTime);

        // Troca de ponto quando atinge o destino
        if (Vector3.Distance(transform.position, proximoPonto) < 0.1f)
        {
            proximoPonto = proximoPonto == pontoA ? pontoB : pontoA;
        }
    }

    private void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Define a plataforma como pai do jogador para que ele acompanhe o movimento
            other.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D (Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Remove o jogador como filho da plataforma ao sair
            other.transform.SetParent(null);
        }
    }

    // Função para desenhar os pontos no editor usando Gizmos
    private void OnDrawGizmos()
    {
        // Define a cor para os pontos
        Gizmos.color = Color.red;

        // Desenha esferas nos pontos A e B
        Gizmos.DrawSphere(pontoA, 0.2f);
        Gizmos.DrawSphere(pontoB, 0.2f);

        // Desenha uma linha entre os pontos A e B
        Gizmos.DrawLine(pontoA, pontoB);
    }
}