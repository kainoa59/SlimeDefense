using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float detectionRange = 5f; // Range to detect enemies
    public Transform basic_black;       // The part of the turret that rotates
    public GameObject projectilePrefab;  // The projectile prefab to instantiate
    public Transform firePoint;          // The point from where the projectile will be fired
    public float fireRate = 2f;          // How often the turret fires (shots per second)
    public float damage = 10f;
    [SerializeField] public float Price = 50f; // Price of the turret

    private float fireCountdown = 0f;    // Timer for firing
    private bool isPlaced = false;        // Indicates if the turret has been placed

    void Update()
    {
        if (!isPlaced) return; // Exit if the turret has not been placed

        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            AimAt(closestEnemy.transform);
            fireCountdown -= Time.deltaTime; // Decrease the countdown timer

            // Check if it's time to fire
            if (fireCountdown <= 0f)
            {
                Fire(closestEnemy); // Pass the closest enemy as a target
                fireCountdown = 1f / fireRate; // Reset the countdown
            }
        }
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemies");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance && distance <= detectionRange)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }

    void AimAt(Transform target)
    {
        Vector3 direction = (target.position - basic_black.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
        basic_black.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void Fire(GameObject target)
    {
        // Instantiate the projectile at the fire point's position
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Set the damage for the projectile based on this turret's damage
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDamage(damage); // Set the damage value for the projectile
        }

        // Get the MoveToPoints script from the enemy
        MoveToPoints moveToPoints = target.GetComponent<MoveToPoints>();
        Vector3 targetVelocity = Vector3.zero;

        if (moveToPoints != null)
        {
            // Use the GetVelocity method to retrieve the velocity
            targetVelocity = moveToPoints.GetVelocity();
        }

        // Calculate the lead position based on the target position and velocity
        Vector3 leadPosition = CalculateLeadPosition(target.transform.position, targetVelocity, projectileScript.Speed);

        // Set the target for the projectile
        if (projectileScript != null)
        {
            projectileScript.SetTarget(leadPosition); // Use the lead position for the projectile
        }

        Debug.Log("Projectile fired towards target: " + target.name); // Log the fire event
    }

    Vector3 CalculateLeadPosition(Vector3 targetPosition, Vector3 targetVelocity, float projectileSpeed)
    {
        Vector3 directionToTarget = targetPosition - firePoint.position;
        float distanceToTarget = directionToTarget.magnitude;
        float timeToReachTarget = distanceToTarget / projectileSpeed;

        // Predict where the target will be after this time
        float leadFactor = 1.5f;
        Vector3 leadPosition = targetPosition + targetVelocity * (timeToReachTarget * leadFactor);
    
        return leadPosition;
    }

    // Call this method when the turret is placed on a base
    public void PlaceTurret()
    {
        isPlaced = true; // Set the turret as placed
    }
}
