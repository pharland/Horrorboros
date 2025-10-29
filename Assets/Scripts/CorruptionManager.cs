using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CorruptionManager : MonoBehaviour
{
    public Slider corruptionBar;
    public float corruptionSpeed;

    internal void DecreaseCorruption(float amountToDecrease)
    {
        corruptionBar.value -= amountToDecrease;
    }

    void Update()
    {
        // increase corruption over time
        if (corruptionBar.value < corruptionBar.maxValue)
        {
            corruptionBar.value += Time.deltaTime * corruptionSpeed;
        }
        else
        {
            // Game Over
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
