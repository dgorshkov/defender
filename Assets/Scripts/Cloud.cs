using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{

    [SerializeField] private Sprite[] cloudSprites;

    private SpriteRenderer theSpriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        theSpriteRenderer = GetComponent<SpriteRenderer>();
        SetCloudSprite();
    }


    private void SetCloudSprite()
    {
        int spriteCount = cloudSprites.Length;
        Sprite spriteToPick = cloudSprites[Random.Range(0, spriteCount-1)];
        theSpriteRenderer.sprite = spriteToPick;
    }
}
