using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PhaseManager : MonoBehaviour
{
    public Image fadeImage; 
    public float fadeDuration = 2f; 
    private CanvasGroup fadeCanvasGroup;

    private void Start()
    {
        fadeCanvasGroup = fadeImage.GetComponent<CanvasGroup>();
        if (fadeCanvasGroup == null)
        {
            fadeCanvasGroup = fadeImage.gameObject.AddComponent<CanvasGroup>();
        }
        fadeCanvasGroup.alpha = 0f; // Começa transparente
        fadeCanvasGroup.blocksRaycasts = false; // Ignora cliques no começo
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
        fadeCanvasGroup.blocksRaycasts = true; // Bloqueia cliques durante o fade out

        while (alpha < 1f)
        {
            alpha += Time.deltaTime / fadeDuration;
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float alpha = 1f;
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / fadeDuration;
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }

        fadeCanvasGroup.blocksRaycasts = false; // Permite cliques novamente
    }
}