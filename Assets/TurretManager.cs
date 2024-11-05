using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurretManager : MonoBehaviour
{
    public GameObject[] turretPrefabs; // Assign different turret prefabs in the inspector
    private GameManager _gameManager; // Reference to GameManager
    private PauseManager _pauseManager; // Reference to PauseManager
    private GameObject currentTurret; // Turret being placed

    [SerializeField] private TMP_Text[] turretCostTexts; // Assign TMP Text components for turret prices in the inspector

    private List<GameObject> placedTurrets = new List<GameObject>(); // List to track placed turrets

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _pauseManager = FindObjectOfType<PauseManager>(); // Find PauseManager in the scene

        // Set the price for the basic tank turret
        if (turretPrefabs.Length > 0) // Ensure there is at least one turret prefab
        {
            Turret basicTankTurret = turretPrefabs[0].GetComponent<Turret>();
            Turret rapidFireTurret = turretPrefabs[1].GetComponent<Turret>();
            if (basicTankTurret != null)
            {
                basicTankTurret.Price = 50.00f; // Set the price to 50
                rapidFireTurret.Price = 75.00f;
                Debug.Log("Basic tank price set to: " + basicTankTurret.Price);
            }
            else
            {
                Debug.LogError("Turret component not found on the basic tank prefab.");
            }
        }
    }

    public void OnPurchaseTurretButtonClicked(int turretIndex) // Pass the turret index when the button is clicked
    {
        Debug.Log($"Button clicked for turret index: {turretIndex}"); // Check if the button was clicked

        if (turretIndex < 0 || turretIndex >= turretPrefabs.Length)
        {
            Debug.LogError("Invalid turret index!");
            return;
        }

        Turret turretScript = turretPrefabs[turretIndex].GetComponent<Turret>(); // Get the Turret component
        Debug.Log("Turret prefab found and Turret script attached.");

        if (turretScript != null)
        {
            float turretCost = turretScript.Price; // Get the turret price
            Debug.Log($"Turret cost: {turretCost}, Player money: {_gameManager.money}");

            if (_gameManager.money >= turretCost)
            {
                _gameManager.AddMoney(-turretCost); // Deduct the cost
                currentTurret = Instantiate(turretPrefabs[turretIndex]); // Instantiate the selected turret prefab
                currentTurret.SetActive(false); // Hide it initially
                Debug.Log("Turret purchased and instantiated: " + turretPrefabs[turretIndex].name);

                // Increase the turret price by 1.5x after each purchase
                turretScript.Price *= 1.5f;
                turretScript.Price = Mathf.Round(turretScript.Price * 100f) / 100f; // Round to 2 decimal places
                Debug.Log($"New Price: {turretScript.Price}");

                // Update the cost display for the specific turret button
                _gameManager.UpdateTurretCostDisplay(turretScript.Price, turretCostTexts[turretIndex]); // Pass the updated cost and specific TMP text
            }
            else
            {
                Debug.Log("Not enough money to purchase turret! Turret " + currentTurret + " costs $" + turretCost);
            }
        }
        else
        {
            Debug.LogError("Turret component not found on the turret prefab.");
        }
    }

    private void Update()
    {
        if (_pauseManager != null && _pauseManager.isPaused)
        {
            return; // Exit if the game is paused
        }

        if (currentTurret != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            currentTurret.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
            currentTurret.SetActive(true); // Ensure the turret is active while moving
        }

        if (Input.GetMouseButtonDown(0) && currentTurret != null)
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
            clickPosition.z = 0;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.CompareTag("PauseButton"))
            {
                _pauseManager.PauseGame();
                return;
            }

            GameObject[] bases = GameObject.FindGameObjectsWithTag("TurretBase");
            GameObject closestBase = null;
            float closestDistance = float.MaxValue;

            foreach (GameObject baseObject in bases)
            {
                float distance = Vector3.Distance(new Vector3(baseObject.transform.position.x, baseObject.transform.position.y, 0), clickPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBase = baseObject;
                }
            }

            if (closestBase != null)
            {
                // Check if the closest base is already occupied
                if (!placedTurrets.Contains(closestBase))
                {
                    // Attach turret to the closest base
                    currentTurret.transform.position = closestBase.transform.position; // Set turret position to the base
                    currentTurret.SetActive(true); // Show the turret

                    // Call the method to indicate the turret is placed
                    Turret turretScript = currentTurret.GetComponent<Turret>();
                    if (turretScript != null)
                    {
                        turretScript.PlaceTurret(); // Allow the turret to start firing
                    }

                    placedTurrets.Add(closestBase); // Add the base to the list of placed turrets
                    currentTurret = null; // Reset current turret
                }
                else
                {
                    Debug.Log("Turret base already occupied!"); // Notify player
                }
            }
        }
    }
}
