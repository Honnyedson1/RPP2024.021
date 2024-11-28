using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dors : MonoBehaviour
{
    private Animator Anim;
    public bool IsOpen;
    private BoxCollider2D box;
    public GameObject a1;
    public GameObject a2;
    
    private void Start()
    {
        a1.SetActive(true);
        a2.SetActive(false);
        Anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();

    }

    private void Update()
    {
        OpenDor();
        if (IsOpen == true)
        {
            a1.SetActive(false);
            a2.SetActive(true);
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