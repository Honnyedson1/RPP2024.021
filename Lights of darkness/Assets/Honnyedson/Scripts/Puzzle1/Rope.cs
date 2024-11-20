using System;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public GameObject box; // Referência à caixa
    private bool isHit = false; // Verifica se a corda já foi atingida
    private Animator Animatora;

    private void Start()
    {
        Animatora = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Flecha") && !isHit)
        {
            HitRope(); 
        }
    }
    private void HitRope()
    {
        Animatora.SetTrigger("HitHop");
        isHit = true; // Marca a corda como atingida
        box.GetComponent<Rigidbody2D>().isKinematic = false; // Permite que a caixa caia
    }
}