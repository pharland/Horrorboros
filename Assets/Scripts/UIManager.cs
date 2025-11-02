using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOverScreen;
    public TextMeshProUGUI segmentsText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;

    [SerializeField] private TailController tailController;

    public void EnableGameOverUI()
    {
        gameOverScreen.SetActive(true);

        // Calculate and show final stats
        segmentsText.text = "Segments: " + tailController.segments.Length + " = " + tailController.segments.Length * 100 + " points";
        timeText.text = "Time: " + Time.timeSinceLevelLoad.ToString("F2") + "s = " + Mathf.FloorToInt(Time.timeSinceLevelLoad * 10) + " points";
        scoreText.text = "Score: " + ((tailController.segments.Length * 100) + Mathf.FloorToInt(Time.timeSinceLevelLoad * 10));
    }
}
