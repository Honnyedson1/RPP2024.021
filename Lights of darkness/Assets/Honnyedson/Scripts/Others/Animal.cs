using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalBehaviour : MonoBehaviour
{
    public Transform player;         // Referência ao Transform do jogador
    public float detectionRange = 5f; // Distância de detecção
    public float speed = 5f;          // Velocidade de corrida do animal

    public Text feedbackText;         // Referência ao campo de texto da UI para mensagens
    public bool isDead = false;       // Controle de estado de vida/morte do animal

    private bool isRunning = false;   // Indica se o animal está fugindo
    private bool collectMessageShown = false;  // Verifica se a mensagem de coleta já foi mostrada
    private Rigidbody2D Rig;
    private Animator Anim;

    private void Start()
    {
        Rig = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead)
        {
            // Verifica se a mensagem de coleta já foi exibida
            if (!collectMessageShown)
            {
                ShowCollectMessage();     // Se o animal está morto, mostra mensagem para coletar
                collectMessageShown = true;  // Marca que a mensagem já foi mostrada
            }
            return;
        }

        // Calcula a distância entre o animal e o jogador
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Se o jogador estiver dentro da área de detecção e o animal ainda não estiver fugindo
        if (distanceToPlayer <= detectionRange && !isRunning)
        {
            Anim.SetBool("IsRun", true);
            isRunning = true;
            ShowChaseMessage();       // Mostra a mensagem para seguir o animal
            Destroy(this.gameObject, 5f);
        }

        // Se o animal estiver fugindo, move-o para a direita
        if (isRunning && !isDead)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }

    // Método que simula a morte do animal ao ser atingido por uma flecha
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Flecha"))  // Verifica se o objeto que colidiu tem a tag "Flecha"
        {
            KillAnimal();  // Chama o método que mata o animal
            Destroy(this.gameObject, 5f);
        }
    }

    // Método que simula a morte do animal
    public void KillAnimal()
    {
        Anim.SetBool("IsRun", false);
        Anim.SetTrigger("Die");
        isDead = true;
        isRunning = false;  // Parar o movimento quando o animal morre
        ShowCollectMessage();
        
    }

    // Mostra a mensagem de coleta
    void ShowCollectMessage()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "Chegue proximo ao animal e coleteo para levar até a vila";
            StartCoroutine(ClearMessageAfterTime(3f));  // Limpa a mensagem após 3 segundos
        }
    }

    // Mostra a mensagem para perseguir o animal
    void ShowChaseMessage()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "O animal está fugindo! Siga-o!";
            StartCoroutine(ClearMessageAfterTime(3f));  // Limpa a mensagem após 3 segundos
        }
    }

    // Corrotina para limpar a mensagem após o tempo determinado
    IEnumerator ClearMessageAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        ClearMessage();
    }

    // Limpa a mensagem da tela
    public void ClearMessage()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    // Desenha a área de detecção no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
