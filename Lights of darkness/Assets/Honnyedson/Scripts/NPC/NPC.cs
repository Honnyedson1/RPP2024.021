using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public Sprite Profile;
    public string[] Speachtext;
    public string NameN;

    public float radius;

    public LayerMask playerlayer;
    public Dialogue Dialo;
    public bool OnRadius;
    private bool dialogueStarted = false;

    private void Start()
    {
        Dialo = FindObjectOfType<Dialogue>();
    }

    private void Update()
    {
        Dialo = FindObjectOfType<Dialogue>();
        if (Input.GetKeyDown(KeyCode.E) && OnRadius && !dialogueStarted)
        {
            Dialo.speach(Profile, Speachtext, NameN);
            dialogueStarted = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && dialogueStarted)
        {
            Dialo.nextsentence();
            if (!Dialo.isActive())
            {
                dialogueStarted = false;
            }
        }
        if (!OnRadius && dialogueStarted)
        {
            Dialo.EndDialogue(); 
            dialogueStarted = false;
        }
    }

    private void FixedUpdate()
    {
        Interactable();
    }

    public void Interactable()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, playerlayer);
        if (hit != null)
        {
            OnRadius = true;
        }
        else
        {
            OnRadius = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
