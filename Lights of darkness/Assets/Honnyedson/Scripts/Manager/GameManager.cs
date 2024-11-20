using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGamePaused;
    public GameObject PauseImage;
    public GameObject Options;
    public GameObject RespawnPanel; // Novo painel de respawn
    [Header("Player Variaveis")]
    public int PlayerDmage = 1;
    public float attackInterval = 1.2f;
    public int Life = 3;
    public int VidaMaxima = 3;
    public int score;
    [Header("CheckPoints")]
    private Vector3 lastCheckpointPosition;
    public GameObject Player;
    public Image ArcoSelected;
    public Image SwordSelected;
    public bool EstouComArco;
    public int QFlechas = 20;
    public bool hasDash = false;
    public bool hasDoubleJump = false;
    public float TimeToNextDesh = 10f;
    public Text LifeText;
    public Text FlechasText;
    public Text Scoretext;
    
    [Header("Dash Indicators")]
    public GameObject DashIndicator1; // Indicador do primeiro dash
    public GameObject DashIndicator2; // Indicador do segundo dash

// Variáveis para salvar o estado do jogador
    private int savedLife;
    private int savedVidaMaxima; // Salva a vida máxima
    private int savedScore;
    private bool savedEstouComArco;
    private int savedQFlechas;
    private bool savedHasDash;
    private bool savedHasDoubleJump;
    private float savedAttackInterval;
    private int savedPlayerDmage; // Corrigido o nome da variável para PlayerDmage


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        ArcoSelected.gameObject.SetActive(false);
        SwordSelected.gameObject.SetActive(true);
        Player = GameObject.FindWithTag("Player");
        lastCheckpointPosition = Player.transform.position;
        RespawnPanel.SetActive(false); // Esconder o painel de respawn
    }

    void Update()
    {
        Player = GameObject.FindWithTag("Player");
        var playerController = Player.GetComponent<PlayerController>();

        if (playerController != null)
        {
            // Atualização dos indicadores de dash
            DashIndicator1.SetActive(hasDash && playerController.canDash1);
            DashIndicator2.SetActive(hasDash && playerController.canDash2);
        }

        // Atualização da interface
        LifeText.text = $"{Life}/{VidaMaxima}";
        FlechasText.text = QFlechas.ToString();
        Scoretext.text = score.ToString();

        if (EstouComArco)
        {
            ArcoSelected.gameObject.SetActive(false);
            SwordSelected.gameObject.SetActive(true);
        }
        else
        {
            ArcoSelected.gameObject.SetActive(true);
            SwordSelected.gameObject.SetActive(false);
        }

        // Alternar pausa com a tecla Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }


    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        SavePlayerState(); // Salva o estado atual do jogador ao alcançar um checkpoint
    }

    // Salva o estado atual do jogador
    private void SavePlayerState()
    {
        savedLife = Life;
        savedVidaMaxima = VidaMaxima; // Salva a vida máxima
        savedScore = score;
        savedEstouComArco = EstouComArco;
        savedQFlechas = QFlechas;
        savedHasDash = hasDash;
        savedHasDoubleJump = hasDoubleJump;
        savedAttackInterval = attackInterval; // Salva o intervalo de ataque
        savedPlayerDmage = PlayerDmage;       // Salva o dano do jogador

        Debug.Log("Estado salvo no checkpoint!");
    }

// Restaura o estado do jogador
    private void RestorePlayerState()
    {
        Life = savedLife;
        VidaMaxima = savedVidaMaxima; // Restaura a vida máxima
        score = savedScore;
        EstouComArco = savedEstouComArco;
        QFlechas = savedQFlechas;
        hasDash = savedHasDash;
        hasDoubleJump = savedHasDoubleJump;
        attackInterval = savedAttackInterval; // Restaura o intervalo de ataque
        PlayerDmage = savedPlayerDmage;       // Restaura o dano do jogador

        // Atualiza os elementos da interface para refletir os valores restaurados
        LifeText.text = $"{Life}/{VidaMaxima}";
        FlechasText.text = QFlechas.ToString();
        Scoretext.text = score.ToString();

        Debug.Log("Estado restaurado no respawn!");
    }

    public void RespawnPlayer()
    {
        Player.transform.position = lastCheckpointPosition; // Reposiciona o jogador
        RestorePlayerState(); // Restaura o estado salvo do jogador
        RespawnPanel.SetActive(false); // Esconde o painel de respawn

        // Libera o jogador para mover-se novamente
        var playerController = Player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetPlayerControl(true); // Habilita o controle do jogador
        }
    }

    public void ShowRespawnPanel()
    {
        if (!RespawnPanel.activeSelf) // Só exibe se ainda não estiver ativo
        {
            RespawnPanel.SetActive(true);

            var playerController = Player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetPlayerControl(false); // Desativa os controles do jogador
            }
        }
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1; // Retorna o tempo normal
        SceneManager.LoadScene(1); // Troca para a cena do menu principal (verifique o índice)
    }

    public void ReturnToCheckpoint()
    {
        RespawnPlayer(); // Respawna o jogador
    }

    public void PauseGame()
    {
        PauseImage.gameObject.SetActive(true);
        isGamePaused = true;
        Time.timeScale = 0;

        // Pausa todos os áudios
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        PauseImage.gameObject.SetActive(false);
        isGamePaused = false;
        Time.timeScale = 1;

        // Retoma todos os áudios
        AudioListener.pause = false;
    }

    public void OptionsGame()
    {
        Options.gameObject.SetActive(true);
        PauseImage.gameObject.SetActive(false);
    }

    public void Back()
    {
        Options.gameObject.SetActive(false);
        PauseImage.gameObject.SetActive(true);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Vilarejo");
    }
}
