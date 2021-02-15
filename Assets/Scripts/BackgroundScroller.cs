using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{

    [SerializeField] float bgScrollSpeed;
    Material myMaterial;
    Vector2 offset;


    // Start is called before the first frame update
    private void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
        offset = new Vector2(0f, bgScrollSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        myMaterial.mainTextureOffset += offset * Time.deltaTime;
    }
}
