using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOverScreen;
    public TextMeshProUGUI segmentsText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;
    [SerializeField] private TailController tailController;
    [SerializeField] private InputActionReference jumpAction; // Assign "Jump" action in Inspector

    private bool isPaused = false;

    public SoundManager soundManager;

    public void EnableGameOverUI()
    {
        // Show game over screen and calculate score
        gameOverScreen.SetActive(true);
        segmentsText.text = "Segments: " + tailController.segments.Length + " = " + tailController.segments.Length * 100 + " points";
        timeText.text = "Time: " + Time.timeSinceLevelLoad.ToString("F2") + "s = " + Mathf.FloorToInt(Time.timeSinceLevelLoad * 10) + " points";
        scoreText.text = "Score: " + ((tailController.segments.Length * 100) + Mathf.FloorToInt(Time.timeSinceLevelLoad * 10));

        // Stop all sound effects and play game over sound effect
        soundManager.StopAllAudio();
        soundManager.PlayGameOverSFX();

        // Pause the game
        Time.timeScale = 0f;
        isPaused = true;
    }

    void Update()
    {
        // Use the new Input System "Jump" action to restart the game
        if (isPaused && jumpAction != null && jumpAction.action.triggered)
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
}
