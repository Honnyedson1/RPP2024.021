using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControler : MonoBehaviour
{
    public Transform player; // A referência para o objeto do jogador
    public float smoothSpeed = 0.125f; // A velocidade suave da câmera
    public Vector3 offset; // O deslocamento da câmera em relação ao jogador

    void FixedUpdate()
    {
        // Calcula a posição ideal da câmera
        Vector3 desiredPosition = new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z);

        // Suaviza a transição da câmera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Atualiza a posição da câmera
        transform.position = smoothedPosition;
    }
}