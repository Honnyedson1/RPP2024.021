using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditTriggerManager2D : MonoBehaviour
{
    public Text[] textSlots; // Textos em três posições diferentes
    public string[] creditMessages; // Mensagens de créditos
    public GameObject finalScreen; // Tela final do jogo

    private int currentCreditIndex = 0;

    private void Start()
    {
        foreach (Text text in textSlots)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }

        if (finalScreen != null)
        {
            finalScreen.SetActive(false);
            Debug.Log("Tela final inicializada como inativa.");
        }
    }

    public void TriggerActivated(GameObject trigger)
    {
        Debug.Log($"Recebido evento de Trigger: {trigger.name}");

        if (currentCreditIndex < creditMessages.Length)
        {
            int textSlotIndex = currentCreditIndex % textSlots.Length;
            Text targetText = textSlots[textSlotIndex];
            StartCoroutine(ShowCredit(targetText, creditMessages[currentCreditIndex]));
            currentCreditIndex++;
        }
        else
        {
            Debug.Log("Mensagens de créditos concluídas. Exibindo tela final.");
            if (finalScreen != null)
                finalScreen.SetActive(true);
        }

        // Desativar o trigger após ser usado
        trigger.SetActive(false);
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
}
