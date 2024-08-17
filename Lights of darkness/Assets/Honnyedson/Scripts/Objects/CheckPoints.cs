using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Salva a posição do checkpoint
            GameManager.Instance.SetCheckpoint(transform.position);
        }
    }
}
