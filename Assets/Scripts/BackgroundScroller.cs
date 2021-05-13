using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{

    [SerializeField] private float bgScrollSpeed;
    private Material myMaterial;
    private Vector2 offset;


    // Start is called before the first frame update
    private void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
        offset = new Vector2(0f, bgScrollSpeed);
    }

    // Update is called once per frame
    private void Update()
    {
        myMaterial.mainTextureOffset += offset * Time.deltaTime;
    }
}
