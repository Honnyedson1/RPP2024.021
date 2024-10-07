using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGamePaused;
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
    }

    private void Update()
    {
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
        Player = GameObject.FindWithTag("Player");
        if (Life <= 0)
        {
            RespawnPlayer();
        }
    }
    
    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
    }
    
    public void RespawnPlayer()
    {
        if (lastCheckpointPosition != Vector3.zero)
        {
            Player.transform.position = lastCheckpointPosition;
        }
        Life = 3; 
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1;
    }

    public void EndGame()
    {
        Debug.Log("Game Over!");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Vilarejo");
    }
}