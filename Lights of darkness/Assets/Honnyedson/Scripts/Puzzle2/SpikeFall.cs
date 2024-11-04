using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeFall : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public float fallSpeed = 5f; // Velocidade de queda
    public float riseSpeed = 3f; // Velocidade para subir
    public float detectionRange = 5f; // Distância para detectar o jogador
    public float fallDistance = 3f; // Distância que o Thwomp cai
    public float waitTime = 1f; // Tempo de espera antes de subir

    private Vector2 originalPosition; // Posição inicial do Thwomp
    private bool isFalling = false; // Verifica se está caindo
    private bool isRising = false; // Verifica se está subindo

    private void Start()
    {
        // Salva a posição inicial
        originalPosition = transform.position;
    }

    private void Update()
    {
        // Calcula a distância entre o jogador e o Thwomp
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Verifica se o jogador está dentro da área de detecção e se o Thwomp não está caindo
        if (distanceToPlayer <= detectionRange && !isFalling && !isRising)
        {
            // Inicia a queda
            isFalling = true;
        }

        // Controle da queda
        if (isFalling)
        {
            transform.position = Vector2.MoveTowards(transform.position, originalPosition - new Vector2(0, fallDistance), fallSpeed * Time.deltaTime);

            // Verifica se atingiu a posição de queda
            if (Vector2.Distance(transform.position, originalPosition - new Vector2(0, fallDistance)) < 0.1f)
            {
                // Espera um tempo e depois começa a subir
                Invoke(nameof(StartRising), waitTime);
                isFalling = false;
            }
        }

        // Controle da subida
        if (isRising)
        {
            transform.position = Vector2.MoveTowards(transform.position, originalPosition, riseSpeed * Time.deltaTime);

            // Verifica se chegou à posição original
            if (Vector2.Distance(transform.position, originalPosition) < 0.1f)
            {
                isRising = false;
            }
        }
    }

    private void StartRising()
    {
        isRising = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha a área de detecção no editor para facilitar ajustes
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}