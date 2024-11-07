using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PhaseManager : MonoBehaviour
{
    public Image fadeImage; 
    public float fadeDuration = 2f; 

    private void Start()
    {
        fadeImage.color = new Color(0f, 0f, 0f, 0f); // Começa transparente
        fadeImage.gameObject.SetActive(true);
    }

    public void TriggerNextPhase()
    {
        StartCoroutine(FadeAndLoadNextScene());
    }

    private IEnumerator FadeAndLoadNextScene()
    {
        yield return FadeOut(); // Escurece a tela
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0); // Volta para a primeira cena
        }

        yield return FadeIn(); // Reaparece a tela gradualmente
    }

    private IEnumerator FadeOut()
    {
        float alpha = 0f;
        fadeImage.gameObject.SetActive(true);

        while (alpha < 1f)
        {
            alpha += Time.deltaTime / fadeDuration; // Aumenta a opacidade
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        fadeImage.gameObject.SetActive(false); // Desativa a imagem após o fade in
    }

    private IEnumerator FadeIn()
    {
        float alpha = 1f;
        fadeImage.gameObject.SetActive(true);

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / fadeDuration; // Diminui a opacidade
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        fadeImage.gameObject.SetActive(false); // Desativa a imagem após o fade in
    }
}