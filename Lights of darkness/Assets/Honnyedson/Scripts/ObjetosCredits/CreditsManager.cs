using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditTriggerManager2D : MonoBehaviour
{
    public Text[] textSlots; // Textos em três posições diferentes
    public string[] creditMessages; // Mensagens de créditos
    public GameObject finalScreen; // Tela final do jogo
    public Image finalScreenBackground; // Imagem de fundo para efeito de fade-in (opcional)

    private bool[] collectedMessages; // Controle de quais mensagens já foram coletadas
    private int currentCreditIndex = 0;

    private void Start()
    {
        // Inicializa o controle de mensagens coletadas
        collectedMessages = new bool[creditMessages.Length];

        foreach (Text text in textSlots)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }

        if (finalScreen != null)
        {
            finalScreen.SetActive(false);
            Debug.Log("Tela final inicializada como inativa.");
        }

        if (finalScreenBackground != null)
        {
            finalScreenBackground.color = new Color(0, 0, 0, 0); // Transparente no início
        }
    }

    public void TriggerActivated(GameObject trigger)
    {
        Debug.Log($"Recebido evento de Trigger: {trigger.name}");

        if (currentCreditIndex < creditMessages.Length)
        {
            // Marca a mensagem como coletada
            collectedMessages[currentCreditIndex] = true;

            int textSlotIndex = currentCreditIndex % textSlots.Length;
            Text targetText = textSlots[textSlotIndex];
            Debug.Log($"Exibindo mensagem de crédito: {creditMessages[currentCreditIndex]}");
            StartCoroutine(ShowCredit(targetText, creditMessages[currentCreditIndex]));
            currentCreditIndex++;
        }

        // Verifica se todas as mensagens foram coletadas
        if (AllMessagesCollected())
        {
            Debug.Log("Todas as mensagens foram coletadas. Aguardando para exibir a tela final.");
            StartCoroutine(ShowFinalScreenAfterDelay(5f));
        }

        // Desativar o trigger após ser usado
        if (trigger != null)
        {
            Debug.Log($"Desativando o trigger: {trigger.name}");
            trigger.SetActive(false);
        }
    }

    private IEnumerator ShowCredit(Text targetText, string message)
    {
        targetText.text = message;

        // Fade in
        for (float alpha = 0; alpha <= 1; alpha += Time.deltaTime)
        {
            targetText.color = new Color(targetText.color.r, targetText.color.g, targetText.color.b, alpha);
            yield return null;
        }

        // Manter o texto visível por 2 segundos
        yield return new WaitForSeconds(2);

        // Fade out
        for (float alpha = 1; alpha >= 0; alpha -= Time.deltaTime)
        {
            targetText.color = new Color(targetText.color.r, targetText.color.g, targetText.color.b, alpha);
            yield return null;
        }

        targetText.text = "";
    }

    private IEnumerator ShowFinalScreenAfterDelay(float delay)
    {
        Debug.Log("Iniciando contagem regressiva para exibir a tela final...");
        yield return new WaitForSeconds(delay);

        // Exibe a tela final com fade-in
        if (finalScreen != null)
        {
            Debug.Log("Exibindo a tela final...");
            finalScreen.SetActive(true);

            if (finalScreenBackground != null)
            {
                Debug.Log("Iniciando fade-in da tela final...");
                for (float alpha = 0; alpha <= 1; alpha += Time.deltaTime)
                {
                    finalScreenBackground.color = new Color(1, 1, 1, alpha);
                    yield return null;
                }
                
            }
        }

        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Menu");
    }

    private bool AllMessagesCollected()
    {
        foreach (bool collected in collectedMessages)
        {
            if (!collected) return false;
        }
        return true;
    }
}
