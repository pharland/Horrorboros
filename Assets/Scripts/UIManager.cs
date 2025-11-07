using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject tutorialScreen;
    public GameObject gameOverScreen;
    public GameObject warningScreen;
    public TextMeshProUGUI segmentsText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;
    [SerializeField] private TailController tailController;
    [SerializeField] private InputActionReference jumpAction; // Assign "Jump" action in Inspector

    private bool isPaused = false;
    private bool isGameOver = false;

    public SoundManager soundManager;

    
    private void Start()
    {
        // one frame after the game starts, show tutorial screen if they haven't seen it before
        if (PlayerPrefs.GetInt("SeenTutorial", 0) == 0)
        {
            Invoke(nameof(ShowTutorialScreen), 0.1f);
            PlayerPrefs.SetInt("SeenTutorial", 1);
            PlayerPrefs.Save();
        }
    }

    private void ShowTutorialScreen()
    {
        tutorialScreen.SetActive(true);
        PauseGame();
    }

    public void EnableGameOverUI()
    {
        // Show game over screen and calculate score
        gameOverScreen.SetActive(true);
        isGameOver = true;
        segmentsText.text = "Segments: " + tailController.segments.Length + " = " + tailController.segments.Length * 100 + " points";
        timeText.text = "Time: " + Time.timeSinceLevelLoad.ToString("F2") + "s = " + Mathf.FloorToInt(Time.timeSinceLevelLoad * 10) + " points";
        scoreText.text = ((tailController.segments.Length * 100) + Mathf.FloorToInt(Time.timeSinceLevelLoad * 10)).ToString();

        // Stop all sound effects and play game over sound effect
        soundManager.StopAllAudio();
        soundManager.PlayGameOverSFX();

        PauseGame();
    }

    public void EnableWarningUI()
    {
        // Show warning screen and pause the game
        warningScreen.SetActive(true);
        PauseGame();
    }

    void Update()
    {
        // Check for jump action to unpause or restart
        if (isPaused && jumpAction != null && jumpAction.action.triggered)
        {
            Time.timeScale = 1f;
            isPaused = false;

            if (isGameOver)
            {
                // Use the new Input System "Jump" action to restart the game
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                // Hide screens
                warningScreen.SetActive(false);
                tutorialScreen.SetActive(false);
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    // Function to clear all PlayerPrefs (for tutorial and audio saves)
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
