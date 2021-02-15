using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{


    [SerializeField] float health = 100;
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed;
    [SerializeField] private GameObject deathEffectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
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
        GameObject projectile = Instantiate(
            projectilePrefab,
            gameObject.transform.position,
            Quaternion.identity);

        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed * -1);
    }





    private void OnTriggerEnter2D(Collider2D otherObject)
    {
        DamageDealer damageDealer = otherObject.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessDamage(damageDealer);
        damageDealer.Remove();
    }

    private void ProcessDamage(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();

        if (health <= 0)
        {
            GameObject deathEffect = Instantiate(
                deathEffectPrefab,
                gameObject.transform.position,
                quaternion.identity);
            deathEffect.transform.Rotate(90f,0f,0F,Space.Self);
            Destroy(deathEffect, 10f);
            Destroy(gameObject);
        }
    }
}
