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
    [Header("Player Variaveis")] public int PlayerDmage = 1;
    public float attackInterval = 1.2f;
    public int Life = 3;
    public int VidaMaxima = 3;
    public int score;
    [Header("CheckPoints")] private Vector3 lastCheckpointPosition;
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

    [Header("Dash Indicators")] public GameObject DashIndicator1; // Indicador do primeiro dash
    public GameObject DashIndicator2; // Indicador do segundo dash

// Variáveis para salvar o estado do jogador
    private int savedLife;
    private int savedVidaMaxima; // Salva a vida máxima
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

        if (Player != null)
            lastCheckpointPosition = Player.transform.position;

        // Garante que a tela de respawn esteja desativada ao iniciar
        if (RespawnPanel != null)
            RespawnPanel.SetActive(false);

        // Atualiza as variáveis de interface
        LifeText.text = $"{Life}/{VidaMaxima}";
        FlechasText.text = QFlechas.ToString();
        Scoretext.text = score.ToString();
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
        savedEstouComArco = EstouComArco;
        savedQFlechas = QFlechas;
        savedHasDash = hasDash;
        savedHasDoubleJump = hasDoubleJump;
        savedAttackInterval = attackInterval; // Salva o intervalo de ataque
        savedPlayerDmage = PlayerDmage; // Salva o dano do jogador

        Debug.Log("Estado salvo no checkpoint!");
    }

// Restaura o estado do jogador
    private void RestorePlayerState()
    {
        Life = savedLife;
        VidaMaxima = savedVidaMaxima; // Restaura a vida máxima
        EstouComArco = savedEstouComArco;
        QFlechas = savedQFlechas;
        hasDash = savedHasDash;
        hasDoubleJump = savedHasDoubleJump;
        attackInterval = savedAttackInterval; // Restaura o intervalo de ataque
        PlayerDmage = savedPlayerDmage; // Restaura o dano do jogador

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
        Time.timeScale = 1;
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

   private void ResetGameManager()
{
    // Reinicia as variáveis do jogador
    PlayerDmage = 1;
    attackInterval = 1.2f;
    VidaMaxima = 3;
    Life = VidaMaxima; // Garante que a vida inicial seja igual à vida máxima
    score = 0;
    EstouComArco = false;
    QFlechas = 20;
    hasDash = false;
    hasDoubleJump = false;
    TimeToNextDesh = 10f;

    // Atualiza a interface para os valores padrão
    LifeText.text = $"{Life}/{VidaMaxima}";
    FlechasText.text = QFlechas.ToString();
    Scoretext.text = score.ToString();

    // Reinicia os indicadores de seleção
    ArcoSelected.gameObject.SetActive(false);
    SwordSelected.gameObject.SetActive(true);

    // Reinicia a posição do checkpoint
    lastCheckpointPosition = Vector3.zero;

    // Garante que o painel de respawn esteja desativado
    if (RespawnPanel != null)
        RespawnPanel.SetActive(false);

    // Reinicia o estado do jogo pausado
    isGamePaused = false;

    Debug.Log("GameManager foi restaurado para os valores iniciais.");
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
        Time.timeScale = 1;
    }

    public void PlayGame()
    {
        ResetGameManager(); // Restaura o GameManager para os valores iniciais
        SceneManager.LoadScene("Vilarejo");
        Time.timeScale = 1;
    }
    
}
