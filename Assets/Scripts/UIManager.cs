using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject controlsPanel;
    public float displayDuration = 5f; // How long to show the controls
    
    private float displayTimer;
    private bool isDisplayingControls;

    void Start()
    {
        // Initially hide the controls panel
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Handle ESC key to quit game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

    public void ShowControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
            isDisplayingControls = true;
            displayTimer = displayDuration;
        }
    }

    private void HideControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
            isDisplayingControls = false;
        }
    }
} 