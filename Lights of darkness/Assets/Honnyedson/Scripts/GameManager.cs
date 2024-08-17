using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Variáveis globais do jogo
    public int score;
    public int playerLives;
    public int coins; // Adicionado para as moedas
    public bool isGamePaused;
    
    [Header("CheckPoints")]
    private Vector3 lastCheckpointPosition;

    public string uiSceneName = "UI";

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

    private void Update()
    {
        if (score >= 3)
        {
            //SceneManager.LoadScene(2);
        }
    }
    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = lastCheckpointPosition;
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

    public void AddScore(int points)
    {
        score += points;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
    }

    public void LoseLife()
    {
        playerLives--;
        if (playerLives <= 0)
        {
            EndGame();
        }
    }
}