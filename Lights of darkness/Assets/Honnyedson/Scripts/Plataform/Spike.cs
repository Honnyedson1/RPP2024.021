using UnityEngine;

public class Espinhos : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var playerHealth = collision.GetComponent<PlayerController>();
        if (playerHealth != null)
        {
            playerHealth.MorteInstantanea();
        }
    }
}
