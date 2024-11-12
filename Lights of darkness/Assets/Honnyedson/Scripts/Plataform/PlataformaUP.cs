using UnityEngine;
using System.Collections.Generic;

public class MovablePlatform : MonoBehaviour
{
    public List<Vector3> pontos;     // Lista de pontos pelos quais a plataforma se moverá
    public float velocidade = 2f;    // Velocidade de movimento

    private int pontoAtual = 0;      // Índice do ponto atual na lista
    private bool moverPlataforma = false; // Flag para iniciar o movimento ao subir
    private Vector3 posicaoInicial;  // Posição inicial para resetar

    private void Start()
    {
        if (pontos.Count < 2)
        {
            Debug.LogError("É necessário definir pelo menos dois pontos para a plataforma se mover.");
            return;
        }

        posicaoInicial = pontos[0];  // Define a posição inicial como o primeiro ponto
        transform.position = posicaoInicial; // Garante que a plataforma comece no ponto inicial
        pontoAtual = 1;  // Define o próximo ponto como o segundo da lista
    }

    private void Update()
    {
        if (moverPlataforma)
        {
            // Move a plataforma para o próximo ponto na lista
            transform.position = Vector3.MoveTowards(transform.position, pontos[pontoAtual], velocidade * Time.deltaTime);

            // Troca de ponto quando atinge o destino
            if (Vector3.Distance(transform.position, pontos[pontoAtual]) < 0.1f)
            {
                pontoAtual = (pontoAtual + 1) % pontos.Count; // Avança para o próximo ponto ou reinicia se for o último
            }
        }

        // Checa se o jogador morreu e reseta a plataforma
        if (GameManager.Instance.Life <= 0)
        {
            ResetarPlataforma();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(transform); // Define a plataforma como pai do jogador
            moverPlataforma = true; // Inicia o movimento ao subir na plataforma
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null); // Remove o jogador como filho da plataforma ao sair
        }
    }

    private void ResetarPlataforma()
    {
        transform.position = posicaoInicial; // Reseta a plataforma para a posição inicial
        pontoAtual = 1;  // Reinicia o próximo ponto a ser alcançado
        moverPlataforma = false; // Pausa o movimento até o jogador subir de novo
    }

    // Função para desenhar os pontos no editor usando Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < pontos.Count; i++)
        {
            Gizmos.DrawSphere(pontos[i], 0.2f);
            if (i < pontos.Count - 1)
            {
                Gizmos.DrawLine(pontos[i], pontos[i + 1]);
            }
        }
    }
}
