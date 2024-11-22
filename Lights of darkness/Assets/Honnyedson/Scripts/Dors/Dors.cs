using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dors : MonoBehaviour
{
    private Animator Anim;
    public bool IsOpen;
    private BoxCollider2D box;
    
    
    private void Start()
    {
        Anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();

    }

    private void Update()
    {
        OpenDor();
        if (IsOpen == true)
        {
            Anim.SetTrigger("OpenDors");
            box.enabled = false;
            IsOpen = false;
        }
    }

    void OpenDor()
    {
        if (BotonDors.Botons >= 3)
        {
            IsOpen = true;
        }
    }
}
