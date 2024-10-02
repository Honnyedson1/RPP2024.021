using UnityEngine;
using UnityEngine.Playables;  // Necessário para o Timeline

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector timeline; // Referência para o Timeline
    public PlayerController playerController; // Referência para o controle do jogador

    void Start()
    {
        // Inscrever-se no evento de término do Timeline
        timeline.stopped += OnCutsceneEnd;
    }

    // Função chamada ao final da Timeline
    void OnCutsceneEnd(PlayableDirector director)
    {
        if (director == timeline)
        {
            // Libera o controle para o jogador
            playerController.SetPlayerControl(true);
        }
    }
}