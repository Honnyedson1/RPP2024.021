using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGamePaused;
    [Header("Player Variaveis")] 
    public int Life = 3;
    public int score;
    [Header("CheckPoints")]
    private Vector3 lastCheckpointPosition;
    public GameObject Player;

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
        Player = GameObject.FindWithTag("Player");
        lastCheckpointPosition = Player.transform.position;
    }

    private void Update()
    {
        if (Life <=0)
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
    
}