using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu; // Assign the PauseMenu in the Inspector
    [SerializeField] private Button pauseButton; // Assign the Pause Button in the Inspector
    [SerializeField] private Button resumeButton; // Assign the Resume Button in the Inspector
    [SerializeField] private Button quitButton; // Assign the Quit Button in the Inspector
    [SerializeField] private Button restartButton;

    public bool isPaused = false;

    void Start()
    {
        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
        restartButton.onClick.AddListener(RestartGame);

        // Ensure the pause menu is hidden at start
        pauseMenu.SetActive(false);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Freeze the game time
        pauseMenu.SetActive(true); // Show the pause menu
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume the game time
        pauseMenu.SetActive(false); // Hide the pause menu
        isPaused = false;
    }

    public void RestartGame()
    {
        // Load the current scene to restart the game
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1f; // Ensure time is resumed
        pauseMenu.SetActive(false);
    }

    public void QuitGame()
    {
        // load a start scene 
        SceneManager.LoadScene("StartScene");
    }
}

