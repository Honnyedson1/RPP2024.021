using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightReduction : MonoBehaviour
{
    public Light2D globalLight; // Referência à luz global Light2D
    public float decreaseSpeed = 0.1f; // Velocidade de diminuição da intensidade da luz
    public float minIntensity = 0.02f; // Intensidade mínima que a luz pode alcançar

    private bool startDimming = false; // Controla se a luz já deve começar a diminuir

    private void Start()
    {
        // Certifique-se de que a luz global foi atribuída, senão tente encontrar uma automaticamente
        if (globalLight == null)
        {
            globalLight = FindObjectOfType<Light2D>();
        }
    }

    private void Update()
    {
        // Se o processo de diminuição tiver começado, continue diminuindo a luz
        if (startDimming && globalLight.intensity > minIntensity)
        {
            globalLight.intensity -= decreaseSpeed * Time.deltaTime;
            if (globalLight.intensity < minIntensity)
            {
                globalLight.intensity = minIntensity; // Garante que a intensidade não caia abaixo do mínimo
            }
        }
    }

    // Quando o jogador encostar, comece a diminuir a intensidade da luz
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            startDimming = true;
        }
    }
}