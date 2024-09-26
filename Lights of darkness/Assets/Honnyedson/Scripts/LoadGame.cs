using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public float TimeToNextScene;
    public string SceneName;
    void Update()
    {
        TimeToNextScene -= Time.deltaTime;
        if (TimeToNextScene <= 0)
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
