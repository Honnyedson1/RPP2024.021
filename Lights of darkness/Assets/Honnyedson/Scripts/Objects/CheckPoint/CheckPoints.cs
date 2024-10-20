using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    private BoxCollider2D cir;

    private void Start()
    {
        cir = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.SetCheckpoint(transform.position);
            cir.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
