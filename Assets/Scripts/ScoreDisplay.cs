using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private GameSession session;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        session = FindObjectOfType<GameSession>();
    }

    // Update is called once per frame
    private void Update()
    {
        scoreText.text = session.GetScore().ToString();
    }
}
