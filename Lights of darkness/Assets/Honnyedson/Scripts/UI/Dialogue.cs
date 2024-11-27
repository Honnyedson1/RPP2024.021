using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public GameObject Dialogueobj;
    public Image Perfil;
    public Text Name;
    public Text SpeachText;

    public float tipyngspeed;
    private string[] sentence;
    private int index;
    private Coroutine typingCoroutine;
    private bool isTyping = false; // Indica se a corrotina de digitação está em execução

    public void speach(Sprite p, string[] txt, string name)
    {
        Dialogueobj.SetActive(true);
        Perfil.sprite = p;
        Name.text = name;
        index = 0; // Resetar o índice ao iniciar um novo diálogo
        sentence = txt;

        // Limpa o texto antes de iniciar a digitação
        SpeachText.text = ""; 

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Interrompe qualquer corrotina de digitação em andamento
        }

        typingCoroutine = StartCoroutine(tipesentence());
    }

    IEnumerator tipesentence()
    {
        isTyping = true; // Marca que a digitação está em andamento
        SpeachText.text = ""; // Garante que o texto seja limpo antes de iniciar a digitação
        foreach (char letter in sentence[index].ToCharArray())
        {
            SpeachText.text += letter;
            yield return new WaitForSeconds(tipyngspeed);
        }
        isTyping = false; // Marca que a digitação foi concluída
    }

    public void nextsentence()
    {
        if (SpeachText.text == sentence[index]) // Se o texto completo já foi exibido
        {
            if (index < sentence.Length - 1)
            {
                index++;
                SpeachText.text = "";

                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine); // Interrompe a corrotina de digitação anterior antes de iniciar a próxima
                }

                typingCoroutine = StartCoroutine(tipesentence());
            }
            else
            {
                EndDialogue();
            }
        }
    }

    public void SkipTyping()
    {
        if (isTyping) // Se o texto ainda está sendo digitado
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine); // Interrompe a corrotina de digitação
            }

            SpeachText.text = sentence[index]; // Exibe o texto completo imediatamente
            isTyping = false; // Atualiza o estado para indicar que a digitação foi concluída
        }
    }

    public bool IsTyping()
    {
        return isTyping; // Retorna se o texto ainda está sendo digitado
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Interrompe a corrotina de digitação ao encerrar o diálogo
        }

        SpeachText.text = "";
        index = 0;
        Dialogueobj.SetActive(false);
    }

    // Verifica se o diálogo ainda está ativo
    public bool isActive()
    {
        return Dialogueobj.activeSelf;
    }
}
