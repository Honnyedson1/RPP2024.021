using UnityEngine;

public class Espinhos : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.Life = 0; 
        }
    }
}
