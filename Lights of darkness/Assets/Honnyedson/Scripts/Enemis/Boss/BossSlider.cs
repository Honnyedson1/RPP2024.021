using UnityEngine;
using UnityEngine.UI;

public class BossHealthSlider : MonoBehaviour
{
    public Slider healthSlider; // Referência ao Slider UI
    private BossController bossController; // Referência ao script BossController
    private bool isActive = false; // Verifica se o slider está ativo
    private bool playerInRange = false; // Se o jogador está dentro do raio de visão

    void Start()
    {
        bossController = GetComponent<BossController>();

        if (bossController == null)
        {
            Debug.LogError("BossController não encontrado no GameObject!");
            return;
        }

        // Inicializa o slider com a vida máxima do boss
        healthSlider.maxValue = bossController.Lifeboss;
        healthSlider.value = bossController.Lifeboss;
        healthSlider.gameObject.SetActive(false); // Desativa o slider no início
    }

    void Update()
    {
        // Verifica a área de detecção do boss
        playerInRange = Physics2D.OverlapCircle(transform.position, bossController.detectionRadius, bossController.playerLayer);

        // Se o jogador entrou na área de visão e está vivo, ativa o slider
        if (playerInRange && GameManager.Instance.Life > 0 && !isActive)
        {
            healthSlider.gameObject.SetActive(true);
            isActive = true;
        }
        // Se o jogador morreu, desativa o slider
        else if (GameManager.Instance.Life <= 0 && isActive)
        {
            healthSlider.gameObject.SetActive(false);
            isActive = false;
        }

        // Atualiza a barra de vida do boss
        if (isActive)
        {
            healthSlider.value = bossController.Lifeboss;
        }
    }

    public void ResetSlider()
    {
        healthSlider.value = bossController.Lifeboss;
        healthSlider.gameObject.SetActive(false);
        isActive = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bossController.detectionRadius); // Mostra o raio de detecção no Editor
    }
}
