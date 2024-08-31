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

    public void speach(Sprite p, string[] txt, string name)
    {
        Dialogueobj.SetActive(true);
        Perfil.sprite = p;
        Name.text = name;
        index = 0; // Resetar o índice ao iniciar um novo diálogo
        sentence = txt;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Interrompe qualquer corrotina de digitação em andamento
        }

        typingCoroutine = StartCoroutine(tipesentence());
    }

    IEnumerator tipesentence()
    {
        SpeachText.text = ""; // Garante que o texto seja limpo antes de iniciar a digitação
        foreach (char letter in sentence[index].ToCharArray())
        {
            SpeachText.text += letter;
            yield return new WaitForSeconds(tipyngspeed);
        }
    }

    public void nextsentence()
    {
        if (SpeachText.text == sentence[index])
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
