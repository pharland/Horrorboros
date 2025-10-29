using UnityEngine;
using UnityEngine.UI;

public class CorruptionManager : MonoBehaviour
{
    public Slider corruptionBar;
    public float corruptionSpeed;
    
    void Update()
    {
        // increase corruption over time
        if (corruptionBar.value < corruptionBar.maxValue)
        {
            corruptionBar.value += Time.deltaTime * corruptionSpeed;
        }
    }
}
