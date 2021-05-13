using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    private GameSession theSession;
    private void Start()
    {
        theSession = FindObjectOfType<GameSession>();
    }
    
    public void LoadStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
        theSession.GameReset();
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    
    public void LoadGameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        theSession.GameReset();
    }
    
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
}
