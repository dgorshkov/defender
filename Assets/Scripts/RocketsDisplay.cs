using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RocketsDisplay : MonoBehaviour
{
    private TextMeshProUGUI rocketText;
    private Player player;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        rocketText = GetComponent<TextMeshProUGUI>();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    private void Update()
    {
        rocketText.text = player.GetCurrentRockets().ToString();
    }
}
