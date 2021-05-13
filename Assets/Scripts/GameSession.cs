using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int scoreGrowth = 1;
    private bool playerAlive = true;
    
    private void Awake()
    {
        SetUpSingleton();
    }

    private void SetUpSingleton()
    {
        int meCount = FindObjectsOfType<GameSession>().Length;
        if (meCount > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    
    
    private void Start()
    {
        StartCoroutine(ConstantScoreGrowth());
    }

    IEnumerator ConstantScoreGrowth()
    {
        while (playerAlive)
        {
            currentScore += scoreGrowth;
            yield return new WaitForSeconds(0.1f);
        }
 
    }

    public void NotifyPlayerDeath()
    {
        playerAlive = false;
    }
    
    public void AddToScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
    }

    public int GetScore()
    {
        return currentScore;
    }

    public void GameReset()
    {
        Destroy(gameObject);
    }
    


}