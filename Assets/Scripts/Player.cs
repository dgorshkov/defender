using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable Unity.PerformanceAnalysis
public class Player : MonoBehaviour
{
    //CONFIG
    
    [Header("Movement")]
    [SerializeField] float moveSpeedX;
    [SerializeField] float moveSpeedY;
    
    //limiting movement to viewport
    float xMin;
    float xMax;
    float yMin;
    float yMax;
    [SerializeField] float minMaxPadding;

    
    [Header("Health")]
    [SerializeField] float startingHealth;
    [SerializeField] float health;
    
    [Header("Basic Attack")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float laserSpeed = 10f;
    [SerializeField] float laserFiringPeriod = 0.1f;

    [Header("VFX")]
    [SerializeField]  GameObject deathEffectPrefab;
    
    [Header("SFX")] 
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip deathSound;

    
    //cached references
    Coroutine firingCoroutine;
    AudioSource theAudioSource;
    


    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
        health = startingHealth;
        theAudioSource = GetComponent<AudioSource>();
    }



    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }

        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }

    }


    
    IEnumerator FireContinuously()
    {
        while (true)
        {
            
            //trigger sound
            theAudioSource.pitch = Random.Range(0.8f, 1.2f);
            theAudioSource.PlayOneShot(attackSound, 0.5f);
            
            
            //create projectile
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);
            yield return new WaitForSeconds(laserFiringPeriod);
        }
        
        // ReSharper disable once IteratorNeverReturns
    }


    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") *moveSpeedX * Time.deltaTime;
        var deltaY = Input.GetAxis("Vertical") * moveSpeedY * Time.deltaTime;


        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);



        transform.position = new Vector2(newXPos, newYPos);
    }



    private void OnTriggerEnter2D(Collider2D otherObject)
    {
        DamageDealer damageDealer = otherObject.gameObject.GetComponent<DamageDealer>();
        ProcessDamage(damageDealer);
        if (!damageDealer) { return; }

        damageDealer.Remove();
    }

    private void ProcessDamage(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();

        if (health <= 0)
        {
            
            //death sound
            Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(deathSound, position, 1.5f);
            
            
            //explosion vfx
            GameObject deathEffect = Instantiate(
                deathEffectPrefab,
                gameObject.transform.position,
                Quaternion.identity);
            deathEffect.transform.Rotate(90f,0f,0F,Space.Self);
            Destroy(deathEffect, 10f);
            
            
            //self destruct
            Destroy(gameObject);
        }
    }

//foofoo


    //limiting movement to viewport
    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + minMaxPadding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - minMaxPadding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + minMaxPadding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - minMaxPadding;
    }
}
