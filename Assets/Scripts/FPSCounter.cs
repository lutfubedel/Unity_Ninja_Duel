using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    private TMP_Text fpsText; // FPS deðeri için UI Text öðesi

    private void Start()
    {
        fpsText = GameObject.FindWithTag("FPS").GetComponent<TextMeshProUGUI>();
        if (fpsText == null)
        {
            Debug.LogError("FPSCounter: Please assign a UI Text element for displaying FPS.");
        }
    }

    private void Update()
    {
        float currentFPS = 1.0f / Time.deltaTime;
        fpsText.text = "FPS: " + Mathf.RoundToInt(currentFPS).ToString();
    }
}
