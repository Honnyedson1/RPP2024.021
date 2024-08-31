using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
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
    private bool dialogueStarted = false; // Para garantir que o diálogo só comece uma vez

    private void Start()
    {
        Dialo = FindObjectOfType<Dialogue>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && OnRadius && !dialogueStarted)
        {
            Dialo.speach(Profile, Speachtext, NameN);
            dialogueStarted = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && dialogueStarted)
        {
            Dialo.nextsentence();
            if (!Dialo.isActive()) // Verifica se o diálogo foi fechado
            {
                dialogueStarted = false;
            }
        }

        // Verifica se o jogador saiu do raio de interação enquanto o diálogo está ativo
        if (!OnRadius && dialogueStarted)
        {
            Dialo.EndDialogue(); // Finaliza o diálogo se o jogador sair do raio
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
=======

public class NPC : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Olá Sou .... Pressione enter para abrir a meu arsenal");
        }
    }
}
>>>>>>> 7bf7f1fcf3f23368a608c949a4ca9503b2ad2f49
