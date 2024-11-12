using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightningFlash : MonoBehaviour
{
    public Light2D globalLight;  // Referência à Global Light 2D
    public AudioClip thunderSound;  // Som do relâmpago
    public float minInterval = 2f;  // Intervalo mínimo entre raios
    public float maxInterval = 5f;  // Intervalo máximo entre raios
    public float rampUpDuration = 0.3f;  // Duração para aumentar a intensidade até o máximo
    public float flashDuration = 0.1f;  // Duração de cada pico de clarão
    public float returnDuration = 0.5f;  // Duração para voltar à intensidade original
    public float maxIntensity = 5.0f;  // Intensidade máxima do clarão do raio
    public int numberOfFlashes = 3;  // Número de flashes durante o clarão

    private float nextFlashTime;
    private float originalIntensity;
    private AudioSource audioSource;

    void Start()
    {
        originalIntensity = globalLight.intensity;  // Salva a intensidade original
        ScheduleNextFlash();
        
        // Configura o AudioSource
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = thunderSound;
    }

    void Update()
    {
        if (Time.time >= nextFlashTime)
        {
            StartCoroutine(FlashLightning());
            ScheduleNextFlash();
        }
    }

    void ScheduleNextFlash()
    {
        nextFlashTime = Time.time + Random.Range(minInterval, maxInterval);
    }

    System.Collections.IEnumerator FlashLightning()
    {
        // Toca o som do relâmpago
        audioSource.Play();

        for (int i = 0; i < numberOfFlashes; i++)
        {
            // Aumenta a intensidade gradualmente
            float elapsed = 0f;
            while (elapsed < rampUpDuration)
            {
                globalLight.intensity = Mathf.Lerp(originalIntensity, maxIntensity, elapsed / rampUpDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Mantém a intensidade no máximo por um tempo
            globalLight.intensity = maxIntensity;
            yield return new WaitForSeconds(flashDuration);

            // Diminui a intensidade rapidamente, simulando o piscar
            elapsed = 0f;
            while (elapsed < flashDuration)
            {
                globalLight.intensity = Mathf.Lerp(maxIntensity, originalIntensity, elapsed / flashDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        // Retorna gradualmente à intensidade original após os flashes
        float returnElapsed = 0f;
        while (returnElapsed < returnDuration)
        {
            globalLight.intensity = Mathf.Lerp(originalIntensity, maxIntensity, 1f - returnElapsed / returnDuration);
            returnElapsed += Time.deltaTime;
            yield return null;
        }

        globalLight.intensity = originalIntensity;
    }
}
