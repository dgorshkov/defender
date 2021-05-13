using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{


    [SerializeField] private float health = 100;
    [SerializeField] private int scoreValue = 100;

    [Header("Shooting")]
    private float shotCounter;
    [SerializeField] private float minTimeBetweenShots = 0.2f;
    [SerializeField] private float maxTimeBetweenShots = 3f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;

    [Header("VFX")] 
    [SerializeField] private Color32 damageFlashColor;
    [SerializeField] private GameObject deathEffectPrefab;
    
    
    [Header("SFX")] 
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;


    [Header("Powerups")] 
    [SerializeField] private GameObject[] possiblePowerups;
    [SerializeField] private float powerupProbability;
    
    
    //cached references
    private AudioSource theAudioSource;
    private GameSession theSession;
    private SpriteRenderer theSpriteRenderer;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        theAudioSource = GetComponent<AudioSource>();
        theSession = FindObjectOfType<GameSession>();
        theSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }


    private void Fire()
    {
        //sfx
        theAudioSource.pitch = Random.Range(0.8f, 1.2f);
        theAudioSource.PlayOneShot(attackSound, 0.1f);
        
        //create and yeet bullet
        GameObject projectile = Instantiate(
            projectilePrefab,
            gameObject.transform.position,
            Quaternion.identity);

        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed * -1);
        

    }





    private void OnTriggerEnter2D(Collider2D otherObject)
    {
        DamageDealer damageDealer = otherObject.gameObject.GetComponent<DamageDealer>();
        if (damageDealer)
        {
            int damageToPass = damageDealer.GetDamage();
            StartCoroutine(ProcessDamage(damageToPass));
            damageDealer.Remove();
        }

        AoeDamageEffect aoeDamageEffect = otherObject.gameObject.GetComponent<AoeDamageEffect>();
        if (aoeDamageEffect)
        {
            //Debug.Log("Enemy collided with AOE effect");
            int aoeDamageToPass = aoeDamageEffect.GetDamage();
            StartCoroutine(ProcessDamage(aoeDamageToPass));
        }

    }

    private IEnumerator ProcessDamage(int damageAmount)
    {

        //reduce health
        health -= damageAmount;

        if (health <= 0)
        {
            Die();
        }
        
        //flash
        theSpriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(0.05f);
        theSpriteRenderer.color = Color.white;
        
    }

    private void Die()
    {
        
        //sfx
        Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        AudioSource.PlayClipAtPoint(deathSound, position, 1f);
        
        
        //vfx
        GameObject deathEffect = Instantiate(
            deathEffectPrefab,
            gameObject.transform.position,
            quaternion.identity);
        deathEffect.transform.Rotate(90f, 0f, 0F, Space.Self);
        Destroy(deathEffect, 10f);
        
        //add score
        theSession.AddToScore(scoreValue);
        
        //spawn powerup maybe
        float roll = Random.value;
        if (roll < powerupProbability)
        {
            int powerUpIndex = Random.Range(0, possiblePowerups.Length);
            GameObject powerUp = Instantiate(
                possiblePowerups[powerUpIndex],
                gameObject.transform.position,
                quaternion.identity);
            powerUp.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -1);
        }
        
        //self destruct
        Destroy(gameObject);
    }
}
