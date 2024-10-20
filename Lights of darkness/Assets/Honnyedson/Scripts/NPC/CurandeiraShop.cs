using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Importação para manipular a interface do usuário

public class ShopNPC : MonoBehaviour
{
    private Image shopUI; // Referência à imagem da loja
    private Button shopButton; // Referência ao botão para abrir a loja
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
            
            // Encontra automaticamente o botão dentro da loja
            Button[] buttons = shopUIObject.GetComponentsInChildren<Button>(true); // Procura por todos os botões, incluindo os inativos
            foreach (Button button in buttons)
            {
                if (button.name == "ShopButton") // Certifique-se de que o botão dentro da loja se chama "ShopButton"
                {
                    shopButton = button;
                    break;
                }
            }

            if (shopButton != null)
            {
                shopButton.gameObject.SetActive(false); // Esconde o botão no início
                shopButton.onClick.AddListener(OpenShop); // Adiciona o evento de clique ao botão
            }
        }
    }

    private void Update()
    {
        Dialo = FindObjectOfType<Dialogue>();

        // Mostra o botão de loja se o jogador estiver na área
        if (OnRadius && !dialogueStarted)
        {
            if (shopButton != null)
            {
                shopButton.gameObject.SetActive(true); // Mostra o botão se o jogador estiver na área
            }
        }
        else
        {
            if (shopButton != null)
            {
                shopButton.gameObject.SetActive(false); // Esconde o botão se o jogador não estiver na área
            }
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

    private void OpenShop()
    {
        bool isActive = shopUI.gameObject.activeSelf;
        shopUI.gameObject.SetActive(!isActive); // Alterna a visibilidade do painel da loja

        if (!isActive)
        {
            Time.timeScale = 0; // Pausa o jogo ao abrir a loja
        }
        else
        {
            Time.timeScale = 1; // Retoma o jogo ao fechar a loja
        }
    }
}
