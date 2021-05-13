using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable Unity.PerformanceAnalysis
public class Player : MonoBehaviour
{
    //CONFIG
    
    [Header("Movement")]
    [SerializeField]
    private float moveSpeedX;
    [SerializeField] private float moveSpeedY;
    
    //limiting movement to viewport
    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;
    [SerializeField] private float minMaxPadding;

    
    [Header("Health")]
    [SerializeField] private float startingHealth;
    [SerializeField] private float health;
    private bool alive = true;
    
    [Header("Basic Attack")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float laserSpeed = 10f;
    [SerializeField] private float laserFiringPeriod = 5f;
    [SerializeField] private float laserSalvoDelay = 0.05f;

    [Header("Secondary Attack")] 
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private float rocketSpeed;
    [SerializeField] private int startingRockets;
    [SerializeField] private int currentRockets = 0;
    
    [Header("VFX")]
    [SerializeField]
    private GameObject deathEffectPrefab;
    
    [Header("SFX")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip secondaryAttackSound;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;

    
    //cached references
    private Coroutine firingCoroutine;
    private AudioSource theAudioSource;
    private SceneLoader theSceneLoader;
    private SpriteRenderer theSpriteRenderer;
    


    // Start is called before the first frame update
    private void Start()
    {
        SetUpMoveBoundaries();
        health = startingHealth;
        currentRockets = startingRockets;
        theAudioSource = GetComponent<AudioSource>();
        theSceneLoader = FindObjectOfType<SceneLoader>();
        theSpriteRenderer = GetComponent<SpriteRenderer>();
    }



    // Update is called once per frame
    private void Update()
    {
        if (alive)
        {
            Move();
            Fire();
            SecondaryAttack();
        }
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


    private IEnumerator FireContinuously()
    {
        while (true)
        {
            
            //sfx
            theAudioSource.pitch = Random.Range(0.95f, 1.05f);
            theAudioSource.PlayOneShot(attackSound, 0.1f);
            
            
            //create projectile
            StartCoroutine(FireSalvo());
            yield return new WaitForSeconds(laserFiringPeriod);
        }
        
        // ReSharper disable once IteratorNeverReturns
    }

    IEnumerator FireSalvo()
    {
        for (int i = 1; i < 4; i++)
        {
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);
            yield return new WaitForSeconds(laserSalvoDelay);
        }
    }

    private void SecondaryAttack()
    {
        if (Input.GetButtonDown("Fire2"))
        {

            PlayerRocket flyingRocket = FindObjectOfType<PlayerRocket>();

            if (flyingRocket)
            {
                flyingRocket.Detonate();
            }
            else if (currentRockets > 0)
            {
                //deduct a rocket
                currentRockets -= 1;
                
                //launch sound
                theAudioSource.PlayOneShot(secondaryAttackSound, 0.1f);
                
                //create and yeet a rocket
                GameObject rocket = Instantiate(rocketPrefab, transform.position, Quaternion.identity);
                rocket.GetComponent<Rigidbody2D>().velocity = new Vector2(0, rocketSpeed);
            }
        }
    }

    public int GetCurrentRockets()
    {
        return currentRockets;
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") *moveSpeedX * Time.deltaTime;
        var deltaY = Input.GetAxis("Vertical") * moveSpeedY * Time.deltaTime;


        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);



        transform.position = new Vector2(newXPos, newYPos);
    }

    public float GetHealth()
    {
        return health;
    }

    private void OnTriggerEnter2D(Collider2D otherObject)
    {
        DamageDealer damageDealer = otherObject.gameObject.GetComponent<DamageDealer>();
        if (damageDealer)
        {
            StartCoroutine(ProcessDamage(damageDealer));
            damageDealer.Remove();
        }
        
        PowerUp powerUp = otherObject.gameObject.GetComponent<PowerUp>();
        if (powerUp)
        {
            StartCoroutine(ProcessPickup(powerUp));
            powerUp.Remove();
        }

    }



    private IEnumerator ProcessDamage(DamageDealer damageDealer)
    {

        //reduce health
        int damage = damageDealer.GetDamage();
        ChangeHealth(damage * -1);
        
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        
        //sound
        theAudioSource.PlayOneShot(damageSound, 0.1f);
        
        //flash
        Color whateverColor = new Color32(255, 0, 0, 255); //edit r,g,b and the alpha values to what you want
        for(var n = 0; n < 3; n++)
        {
            theSpriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.05f);
            theSpriteRenderer.color = whateverColor;
            yield return new WaitForSeconds(0.05f);
        }
        theSpriteRenderer.color = Color.white;
        
    }

    private IEnumerator Die()
    {
        //death sound
        Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        AudioSource.PlayClipAtPoint(deathSound, position, 1f);


        //explosion vfx
        GameObject deathEffect = Instantiate(
            deathEffectPrefab,
            gameObject.transform.position,
            Quaternion.identity);
        deathEffect.transform.Rotate(90f, 0f, 0F, Space.Self);
        Destroy(deathEffect, 10f);

        //set alive flag
        alive = false;

        //hide itself
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        
        //remove children
        ClearChildren();
        
        //notify GameSession
        FindObjectOfType<GameSession>().NotifyPlayerDeath();
        
        //load game over
        yield return new WaitForSeconds(2f);
        theSceneLoader.LoadGameOver();
        
    }
    
    public void ClearChildren()
    {
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            Destroy(child.gameObject);
        }
        
    }
    
    private IEnumerator ProcessPickup(PowerUp powerUp)
    {
        //sound
        theAudioSource.PlayOneShot(pickupSound, 0.2f);
        
        //get buffs
        int extraHealth = powerUp.GetHealthEffect();
        ChangeHealth(extraHealth);
        currentRockets += powerUp.GetSecondaryAttackEffect();
        
        //flash
        Color whateverColor = new Color32(0, 255, 14, 255); //edit r,g,b and the alpha values to what you want
        for(var n = 0; n < 3; n++)
        {
            theSpriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.05f);
            theSpriteRenderer.color = whateverColor;
            yield return new WaitForSeconds(0.05f);
        }
        theSpriteRenderer.color = Color.white;
    }


    private void ChangeHealth(int change)
    {
        health += change;
        health = Mathf.Clamp(health, 0, startingHealth);
    }

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
