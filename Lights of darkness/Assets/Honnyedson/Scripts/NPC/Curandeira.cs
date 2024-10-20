using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Importação para controle de fases

public class Curandeira : MonoBehaviour
{
    public Sprite Profile;
    public string[] Speachtext;
    public string NameN;
    public PhaseManager phaseManager;
    public float radius;

    public LayerMask playerlayer;
    public Dialogue Dialo;
    public bool OnRadius;
    private bool dialogueStarted = false;
    private PlayerController playerMovement; // Referência ao script de movimento do jogador

    private void Start()
    {
        phaseManager = FindObjectOfType<PhaseManager>();
        Dialo = FindObjectOfType<Dialogue>();
        playerMovement = FindObjectOfType<PlayerController>(); // Busca o script de movimento do jogador
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && OnRadius && !dialogueStarted)
        {
            Dialo.speach(Profile, Speachtext, NameN);
            dialogueStarted = true;
            playerMovement.canMove = false;
            playerMovement.rb.velocity = Vector2.zero;
            playerMovement.anim.SetInteger("Transition", 0);
        }

        if (Input.GetKeyDown(KeyCode.E) && dialogueStarted)
        {
            Dialo.nextsentence();
            if (!Dialo.isActive())
            {
                dialogueStarted = false;
                playerMovement.canMove = true;
                phaseManager.TriggerNextPhase(); 
            }
        }

        if (!OnRadius && dialogueStarted)
        {
            Dialo.EndDialogue();
            dialogueStarted = false;
            playerMovement.enabled = true; // Reabilita o movimento do jogador caso saia do raio do NPC
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
