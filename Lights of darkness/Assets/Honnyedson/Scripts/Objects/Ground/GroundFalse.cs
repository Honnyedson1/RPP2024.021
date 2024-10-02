using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFalse : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            gameObject.SetActive(false);
        }
    }
}
