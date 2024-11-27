using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private void Update()
    {
        // Verifica se o Time.timeScale está em 0 (pausado)
        if (Time.timeScale == 0)
        {
            AudioListener.pause = true; // Pausa todos os sons
        }
        else if (Time.timeScale == 1)
        {
            AudioListener.pause = false; // Retoma os sons
        }
    }
}