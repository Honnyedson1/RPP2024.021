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

    public Text LifeText;
    public Text FlechasText;
    public Text Scoretext;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

        LifeText.text = Life.ToString();
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

        if (Life <= 0)
        {
            ShowRespawnPanel();
        }
    }
    
    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
    }
    
    public void RespawnPlayer()
    {
        Player.transform.position = lastCheckpointPosition; // Reposiciona o jogador
        Life = VidaMaxima; // Restaura a vida ao máximo
        RespawnPanel.SetActive(false); // Esconde o painel de respawn
        Time.timeScale = 1; // Retorna o tempo normal
    }

    private void ShowRespawnPanel()
    {
        RespawnPanel.SetActive(true); // Mostra o painel de respawn
        Time.timeScale = 0; // Pausa o jogo
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
    }

    public void ResumeGame()
    {
        PauseImage.gameObject.SetActive(false);
        isGamePaused = false;
        Time.timeScale = 1;
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
