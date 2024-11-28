using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dors : MonoBehaviour
{
    private Animator Anim;
    public bool IsOpen;
    private BoxCollider2D box;
    public GameObject a1;
    public Text mensagemTexto; // Referência ao componente Text da UI
    private bool mensagemExibida = false;
    public GameObject a2;
    public bool jaaaa;

    private void Start()
    {
        mensagemTexto.gameObject.SetActive(false); // Inicialmente, a mensagem não será visível
        a1.SetActive(true);
        a2.SetActive(false);
        Anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();

    }

    private void Update()
    {
        OpenDor();
        if (IsOpen == true)
        {
            a1.SetActive(false);
            a2.SetActive(true);
            Anim.SetTrigger("OpenDors");
            if (jaaaa == false)
            {
                StartCoroutine(ExibirMensagem());
            }
            
            box.enabled = false;
            IsOpen = false;
        }
    } 
    private IEnumerator ExibirMensagem()
    {
        Debug.Log("AS");
        // Exibe a mensagem na tela
        mensagemTexto.gameObject.SetActive(true);
        mensagemTexto.text = "Os cristais ergueram a porta. Volte para sair da caverna.";

        // Aguarda 3 segundos
        yield return new WaitForSeconds(3f);

        // Após 3 segundos, a mensagem desaparece
        mensagemTexto.gameObject.SetActive(false);
        jaaaa = true;
    }
    void OpenDor()
    {
        if (BotonDors.Botons >= 3)
        {
            IsOpen = true;
        }
    }
}