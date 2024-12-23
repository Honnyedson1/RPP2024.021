using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopNPC : MonoBehaviour
{
    public Sprite Profile;
    public string[] Speachtext;
    public string NameN;

    public float radius;
    public LayerMask playerLayer;
    public Dialogue Dialo; // Referência ao sistema de diálogo
    public bool OnRadius;
    private bool dialogueStarted = false;

    private void Start()
    {
        Dialo = FindObjectOfType<Dialogue>();
    }

    private void Update()
    {
        Dialo = FindObjectOfType<Dialogue>();

        // Lógica para iniciar o diálogo
        if (Input.GetKeyDown(KeyCode.E) && OnRadius && !dialogueStarted)
        {
            Dialo.speach(Profile, Speachtext, NameN);
            dialogueStarted = true; // Inicia o diálogo
        }

        if (Input.GetKeyDown(KeyCode.E) && dialogueStarted)
        {
            if (Dialo.IsTyping()) // Verifica se o texto ainda está sendo digitado
            {
                Dialo.SkipTyping(); // Pula a digitação
            }
            else
            {
                Dialo.nextsentence(); // Vai para a próxima sentença
            }

            if (!Dialo.isActive())
            {
                dialogueStarted = false; // Finaliza o diálogo
            }
        }
        // Finaliza o diálogo ao sair do raio
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
        Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, playerLayer);
        OnRadius = hit != null; // Atualiza o estado de OnRadius
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}