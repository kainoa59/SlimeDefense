using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f; // Max health for the enemy
    private float currentHealth; // Current health of the enemy
    private AudioSource _audioSource;

    [SerializeField] private GameObject healthBarCanvas; // Assign the Health Bar Canvas
    [SerializeField] private RectTransform healthBarFill; // Assign the RectTransform of the GreenFill image
    [SerializeField] private float moneyValue = 10f; // Money value for the enemy

    private GameManager _gameManager; // Reference to GameManager
    private MoveToPoints _moveToPoints; // Reference to MoveToPoints

    private void Start()
    {
        ResetHealth(); // Initialize health
        _gameManager = FindObjectOfType<GameManager>(); // Find the GameManager instance
        _moveToPoints = GetComponent<MoveToPoints>(); // Get the MoveToPoints component
        _audioSource = GetComponent<AudioSource>(); // Assuming AudioSource is on the same GameObject
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Current Health: " + currentHealth); // Log current health
        if (currentHealth < 0) currentHealth = 0; // Ensure health doesn't go below zero
        UpdateHealthBar();

        // Check if the enemy should be destroyed
        if (currentHealth <= 0)
        {
            Debug.Log("Enemy died, playing death sound."); // Debug log for death
            PlayDeathSound();
            
            // Reset currentPointIndex to 0 before returning to pool
            if (_moveToPoints != null)
            {
                _moveToPoints.ResetCurrentPointIndex(); // Reset the index to 0
            }

            // Call the method to add money to the player's total
            if (_gameManager != null)
            {
                _gameManager.AddMoney(moneyValue); // Add money value to the GameManager
            }
            
            ObjectPooler.ReturnToPool(gameObject); // Destroy the enemy
        }
    }

    private void Die()
    {
        PlayDeathSound(); // Play death sound

        // Reset currentPointIndex to 0 before returning to pool
        if (_moveToPoints != null)
        {
            _moveToPoints.ResetCurrentPointIndex(); // Reset the index to 0
        }

        // Call the method to add money to the player's total
        if (_gameManager != null)
        {
            _gameManager.AddMoney(moneyValue); // Add money value to the GameManager
        }

        ObjectPooler.ReturnToPool(gameObject); // Destroy the enemy
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth; // Reset current health to max health
        UpdateHealthBar(); // Update the health bar to reflect the new health
    }

    public void IncreaseMaxHealth(float multiplier)
    {
        maxHealth *= multiplier; // Multiply max health by the specified multiplier
        maxHealth = Mathf.Round(maxHealth * 100f) / 100f; // Round to 2 decimal places
        ResetHealth(); // Reset current health to the new max
    }

    public void IncreaseMoneyValue(float multiplier)
    {
        moneyValue *= multiplier; // Multiply money value by the specified multiplier
        moneyValue = Mathf.Round(moneyValue * 100f) / 100f; // Round to 2 decimal places
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            // Calculate the new width based on the current health
            float healthPercentage = currentHealth / maxHealth; // Get the health percentage (0 to 1)
            float fillWidth = healthPercentage * 0.41f; // Scale it by the starting width (0.41)

            // Set the sizeDelta on the RectTransform
            healthBarFill.sizeDelta = new Vector2(fillWidth, healthBarFill.sizeDelta.y);
        }
    }

    private void PlayDeathSound()
    {
        if (_audioSource != null && _audioSource.clip != null)
        {
            Debug.Log("Playing death sound.");
            _audioSource.Play();
            
            // Check if the audio source is currently playing
            if (!_audioSource.isPlaying)
            {
                Debug.LogWarning("Death sound did not play.");
            }
        }
        else
        {
            Debug.LogWarning("AudioSource is null or no clip assigned!"); // Debug message
        }
    }
}
