using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Necessário para usar coroutines

public class ControlTutorial : MonoBehaviour
{
    public Text tutorialText; // Arraste o texto do Canvas aqui no Inspector

    private void Start()
    {
        tutorialText.text = "Precione A ou D para se movimentar "; // Começa sem texto
    }

    public void DisplayMessage(string message)
    {
        tutorialText.text = message; // Exibe a mensagem recebida
        StartCoroutine(HideMessageAfterDelay(4f)); // Chama a coroutine para esconder a mensagem após 4 segundos
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Espera pelo tempo especificado
        tutorialText.text = ""; // Limpa o texto
    }
}