using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BotonDors : MonoBehaviour
{
    [Serialize] public static int Botons;
    public Animator Torch;

    private void Start()
    {
        Torch = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Flecha")
        {
            Torch.SetTrigger("Abriu");
            Botons++;
        }
    }
}
