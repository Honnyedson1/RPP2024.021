using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.Tab; // Tecla para abrir/fechar o painel
    public ShopNPC npc; // Referência ao NPC para verificar a proximidade do jogador
    public Image shopUI; // Referência ao painel da loja
    // Procura o painel de loja pela tag, mesmo que esteja inicialmente desativado
    public GameObject shopUIPanel;
    private void Start()
    {
        shopUIPanel = FindInactiveObjectByTag("ShopUI");
        if (shopUIPanel != null)
        {
            shopUI = shopUIPanel.GetComponent<Image>();
            shopUI.gameObject.SetActive(false); // Certifica-se de que a loja começa fechada
        }
        else
        {
            Debug.LogError("Nenhum painel de loja encontrado com a tag 'ShopUI'.");
        }

        if (npc == null)
        {
            Debug.LogError("NPC não atribuído ao sistema de loja.");
        }
    }

    private void Update()
    {
        if (shopUI == null)
        {
            shopUIPanel = FindInactiveObjectByTag("ShopUI");
            if (shopUIPanel != null)
            {
                shopUI = shopUIPanel.GetComponent<Image>();
                shopUI.gameObject.SetActive(false); // Certifica-se de que a loja começa fechada
            }
            else
            {
                Debug.LogError("Nenhum painel de loja encontrado com a tag 'ShopUI'.");
            }
        }
        // Controle de abertura/fechamento da loja com a tecla configurada
        if (Input.GetKeyDown(toggleKey) && npc != null && npc.OnRadius)
        {
            ToggleShop();
        }
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

    // Método para encontrar objetos inativos por tag
    private GameObject FindInactiveObjectByTag(string tag)
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in allTransforms)
        {
            if (t.CompareTag(tag) && t.gameObject.hideFlags == HideFlags.None)
            {
                return t.gameObject;
            }
        }
        return null;
    }
}
