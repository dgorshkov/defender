using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRocket : MonoBehaviour
{

    [SerializeField] private GameObject rocketExplosionPrefab;
    [SerializeField] private AudioClip deathSound;
    

    private void OnTriggerEnter2D(Collider2D otherObject)
    {
        Detonate();
    }

    public void Detonate()
    {
        //create explosion
        GameObject explosion = Instantiate(rocketExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 0.4f);
        
        //sound
        Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        AudioSource.PlayClipAtPoint(deathSound, position, 1f);
        
        //self destruct
        Destroy(gameObject);
    }
    

}
