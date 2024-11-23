using UnityEngine;
using UnityEngine.UI;

public class BossHealthSlider : MonoBehaviour
{
    public Slider healthSlider; // Referência ao Slider UI
    private BossController bossController; // Referência ao script BossController
    private bool isActive = false; // Verifica se o slider está ativo

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
        // Verifica se a vida do jogador chegou a zero
        if (GameManager.Instance.Life > 0 && !isActive)
        {
            healthSlider.gameObject.SetActive(true);
            isActive = true;
        }
        else if (GameManager.Instance.Life <= 0 && isActive)
        {
            healthSlider.gameObject.SetActive(false);
            isActive = false;
        }

        // Atualiza a barra de vida com o valor atual da vida do boss
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
}