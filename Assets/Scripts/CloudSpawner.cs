using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{

    [SerializeField] private GameObject cloudPrefab;
    [SerializeField] private float spawnTime = 10f;
    [SerializeField] private float spawnTimeRandomness = 5f;
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;
    [SerializeField] private float cloudSpeed = 1f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreateClouds());
    }

    private IEnumerator CreateClouds()
    {
        while (true)
        {
            //spawn
            Vector3 position = new Vector3(Random.Range(minX, maxX), 8, 0.5f);
            GameObject newCloud = Instantiate(cloudPrefab, position, Quaternion.identity);
            
            //start moving
            float speed = cloudSpeed + Random.Range(cloudSpeed * 0.1f, cloudSpeed * 0.2f);
            newCloud.GetComponent<Rigidbody2D>().velocity = new Vector2(0, speed * -1);

            //wait
            float delay = spawnTime + Random.Range(0, spawnTimeRandomness);
            yield return new WaitForSeconds(delay);
        }
        
    }
    
    
}
