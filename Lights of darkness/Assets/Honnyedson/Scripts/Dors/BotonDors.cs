using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BotonDors : MonoBehaviour
{
    [Serialize] public static int Botons;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Flecha")
        {
            Botons++;
        }
    }
}
