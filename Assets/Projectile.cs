using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifespan = 3f; // Time in seconds before the projectile is destroyed
    [SerializeField] private float speed = 10f; // Speed of the projectile
    [SerializeField] private AudioClip fireSound; // The sound to play when the projectile is fired

    private Vector3 direction;
    private float damage; // Damage dealt by this projectile

    void Start()
    {
        // Play the fire sound
        PlayFireSound();

        // Destroy the projectile after a certain time
        Destroy(gameObject, lifespan);
    }

    private void PlayFireSound()
    {
        // Create a temporary GameObject to play the sound
        GameObject soundObject = new GameObject("ProjectileSound");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = fireSound;

        audioSource.Play();
        // Destroy the temporary GameObject after the audio clip finishes
        Destroy(soundObject, audioSource.clip.length);
    }

    public void SetTarget(Vector3 targetPosition)
    {
        // Calculate direction toward the target
        direction = (targetPosition - transform.position).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * speed; // Set velocity in the direction of the target
        }
        // Rotate the projectile to face the target direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void SetDamage(float newDamage) // Method to set damage
    {
        damage = newDamage; // Set the damage value
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("TurretBase"))
        {
            return; // Ignore collisions with turret bases
        }

        if (collision.gameObject.CompareTag("enemies"))
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage); // Deal the specified damage
            }

            Destroy(gameObject); // Destroy the projectile after hit
        }
    }

    public float Speed
    {
        get { return speed; }
    }
}
