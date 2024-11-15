using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopNPC : MonoBehaviour
{
    public Image shopUI; // Referência ao painel da loja
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

        // Encontra automaticamente o painel da loja pelo nome
        GameObject shopUIObject = GameObject.Find("ShopUI"); // Encontre o GameObject que contém a Image
        if (shopUIObject != null)
        {
            shopUI = shopUIObject.GetComponent<Image>(); // Obtém a referência ao componente Image
            shopUIObject.SetActive(false); // Certifica-se de que a loja começa fechada
        }
        else
        {
            Debug.LogError("Painel 'ShopUI' não encontrado na cena.");
        }
    }

    private void Update()
    {
        Dialo = FindObjectOfType<Dialogue>();

        // Controle de abertura/fechamento da loja com a tecla TAB
        if (OnRadius && Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleShop(); // Alterna entre abrir e fechar a loja
        }

        // Lógica para o diálogo
        if (Input.GetKeyDown(KeyCode.E) && OnRadius && !dialogueStarted)
        {
            Dialo.speach(Profile, Speachtext, NameN);
            dialogueStarted = true; // Inicia o diálogo
        }

        if (Input.GetKeyDown(KeyCode.E) && dialogueStarted)
        {
            Dialo.nextsentence();
            if (!Dialo.isActive())
            {
                dialogueStarted = false; // Finaliza o diálogo
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
        Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, playerLayer);
        OnRadius = hit != null; // Atualiza o estado de OnRadius
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void ToggleShop()
    {
        if (shopUI != null)
        {
            bool isActive = shopUI.gameObject.activeSelf;
            shopUI.gameObject.SetActive(!isActive); // Alterna a visibilidade do painel da loja

            // Pausa ou retoma o jogo dependendo do estado da loja
            Time.timeScale = isActive ? 1 : 0;
        }
    }
}
