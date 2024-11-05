using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For Button
using TMPro; // For TMP elements

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject tutorialCanvas; // Tutorial Canvas 
    [SerializeField] private Button startButton; // Start Button 
    [SerializeField] private Button tutorialButton; // Tutorial Button 
    [SerializeField] private Button closeTutorialButton; // Close Button 

    void Start()
    {
        // Add listeners to buttons
        startButton.onClick.AddListener(StartGame);
        tutorialButton.onClick.AddListener(OpenTutorial);
        closeTutorialButton.onClick.AddListener(CloseTutorial);
        
        // Ensure the tutorial canvas is hidden at start
        tutorialCanvas.SetActive(false);
    }

    private void StartGame()
    {
        // Load the main game scene (SampleScene)
        SceneManager.LoadScene("SampleScene");
    }

    private void OpenTutorial()
    {
        // Show the tutorial canvas
        tutorialCanvas.SetActive(true);
    }

    private void CloseTutorial()
    {
        // Hide the tutorial canvas
        tutorialCanvas.SetActive(false);
    }
}
