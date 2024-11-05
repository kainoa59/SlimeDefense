using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int playerHealth = 20;
    public float money = 50;
    public TMP_Text playerStatsText;
    public TMP_Text turretCostText;

    void Start()
    {
        UpdatePlayerStats();
    }

    public void DeductHealth(int amount)
    {
        playerHealth -= amount;
        UpdatePlayerStats();

        if (playerHealth <= 0)
        {
            Debug.Log("Game Over!");
            GameOver();
        }
    }

    void GameOver()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void AddMoney(float amount)
    {
        money += amount;
        UpdatePlayerStats();
    }

    private void UpdatePlayerStats()
    {
        playerStatsText.text = $"Player Health: {playerHealth}\n\nMoney: ${money}";
    }

    public void UpdateTurretCostDisplay(float newCost, TMP_Text turretCostText)
    {
        turretCostText.text = $"Turret Cost: ${newCost:F2}"; // Display updated cost with 2 decimal places
    }
}
