using System;
using UnityEngine;

public class DoorBox : MonoBehaviour
{
    public GameObject doorObject; // Referência ao objeto da porta
    private bool isOpen = false; // Status da porta (aberta ou fechada)

    private void Start()
    {
        // Certifique-se de que a porta esteja fechada no início
        doorObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se a caixa entrou no colisor
        if (other.CompareTag("Box") || other.CompareTag("Player"))
        {
            OpenDoor(); // Abre a porta
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Verifica se a caixa saiu do colisor
        if (other.CompareTag("Box") || other.CompareTag("Player"))
        {
            CloseDoor(); // Fecha a porta
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true; // Marca a porta como aberta
            doorObject.SetActive(false); // Desativa a porta (aberta)
            Debug.Log("A porta se abriu!");
        }
    }

    private void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false; // Marca a porta como fechada
            doorObject.SetActive(true); // Ativa a porta (fechada)
            Debug.Log("A porta se fechou!");
        }
    }
}