using System;
using UnityEngine;

public class DoorBox : MonoBehaviour
{
    public GameObject doorObject; // Referência ao objeto da porta
    private bool isOpen = false; // Status da porta (aberta ou fechada)
    public bool Caixaemcima;

    private void Start()
    {
        // Certifique-se de que a porta esteja fechada no início
        doorObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            doorObject.GetComponent<Animator>().SetBool("IsOpen", true);
            OpenDoor();
            Caixaemcima = true;
        }

        if (other.CompareTag("Player"))
        {
            if (Caixaemcima == false)
            {
                doorObject.GetComponent<Animator>().SetBool("IsOpen", true);
                OpenDoor();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Caixaemcima == false)
            {
                doorObject.GetComponent<Animator>().SetBool("IsOpen", false);
                CloseDoor(); 
            }
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
            doorObject.GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log("A porta se abriu!");
        }
    }

    private void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false; // Marca a porta como fechada
            doorObject.GetComponent<BoxCollider2D>().enabled = true;
            Debug.Log("A porta se fechou!");
        }
    }
}