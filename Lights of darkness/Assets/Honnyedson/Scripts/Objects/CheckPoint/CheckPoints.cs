using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    private CircleCollider2D cir;

    private void Start()
    {
        cir = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.SetCheckpoint(transform.position);
            cir.GetComponent<CircleCollider2D>().enabled = false;
        }
    }
}
