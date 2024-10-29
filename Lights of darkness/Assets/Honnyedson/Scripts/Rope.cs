using UnityEngine;

public class Rope : MonoBehaviour
{
    public GameObject box; // Referência à caixa
    private bool isHit = false; // Verifica se a corda já foi atingida

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Flecha") && !isHit)
        {
            HitRope(); 
        }
    }
    private void HitRope()
    {
        isHit = true; // Marca a corda como atingida
        box.GetComponent<Rigidbody2D>().isKinematic = false; // Permite que a caixa caia
        Debug.Log("A caixa caiu!");
    }
}