using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeFall : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public float fallSpeed = 5f; // Velocidade de queda
    public float riseSpeed = 3f; // Velocidade para subir
    public float detectionRange = 5f; // Largura da área de detecção
    public float detectionHeight = 2f; // Altura da área de detecção
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
        if (IsPlayerWithinBox())
        {
            // Inicia a queda
            if (!isFalling && !isRising)
            {
                isFalling = true;
            }
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

    private bool IsPlayerWithinBox()
    {
        // Verifica se o jogador está dentro de uma área retangular
        Vector2 boxCenter = new Vector2(transform.position.x, transform.position.y - detectionHeight / 2);
        return player.position.x > boxCenter.x - detectionRange / 2 &&
               player.position.x < boxCenter.x + detectionRange / 2 &&
               player.position.y > boxCenter.y - detectionHeight / 2 &&
               player.position.y < boxCenter.y + detectionHeight / 2;
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha a área de detecção no editor para facilitar ajustes
        Gizmos.color = Color.red;
        Vector3 boxSize = new Vector3(detectionRange, detectionHeight, 0);
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y - detectionHeight / 2, transform.position.z), boxSize);
    }
}
