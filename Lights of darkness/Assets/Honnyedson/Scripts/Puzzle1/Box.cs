using UnityEngine;

public class Box : MonoBehaviour
{
    // Adicione este script à caixa

    private void Start()
    {
        // Define a caixa como cinemática inicialmente
        GetComponent<Rigidbody2D>().isKinematic = true; // A caixa não pode cair no início
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Se a caixa colidir com o chão ou um colisor específico
        if (collision.gameObject.CompareTag("Ground"))
        {
            GetComponent<Rigidbody2D>().isKinematic = false; // Permite que a caixa caia
        }
    }
}