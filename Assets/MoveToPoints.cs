using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPoints : MonoBehaviour
{
    public Waypoints waypoints;
    public float speed = 1f;
    private int currentPointIndex = 0;
    private ObjectPooler _pooler;
    private GameManager _gameManager;
    private AudioSource _audioSource; // Reference to AudioSource

    void Start()
    {
        _pooler = FindObjectOfType<ObjectPooler>();
        _gameManager = FindObjectOfType<GameManager>();
        _audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        if (waypoints == null || waypoints.Points.Length == 0)
        {
            Debug.LogError("Waypoints not set");
            return;
        }
    }

    void Update()
    {
        if (waypoints.Points.Length == 0) return;

        Vector3 targetPosition = waypoints.GetWaypointPosition(currentPointIndex);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % waypoints.Points.Length;
        }

        if (Vector3.Distance(transform.position, waypoints.GetWaypointPosition(waypoints.Points.Length - 1)) < 0.1f)
        {
            // Play the death sound
            PlayDeathSound();

            ObjectPooler.ReturnToPool(gameObject);
            _gameManager?.DeductHealth(1);
        }
    }

    // Method to play the death sound
    private void PlayDeathSound()
    {
        if (_audioSource != null)
        {
            Debug.Log("Playing death sound");
            _audioSource.Play(); // Play the sound
        }
        else
        {
            Debug.LogWarning("AudioSource is null!");
        }
    }

    public Vector3 GetVelocity()
    {
        if (waypoints.Points.Length == 0) return Vector3.zero;

        Vector3 targetPosition = waypoints.GetWaypointPosition(currentPointIndex);
        Vector3 direction = (targetPosition - transform.position).normalized;

        return direction * speed;
    }

    public void ResetCurrentPointIndex()
    {
        currentPointIndex = 0;
    }
}
