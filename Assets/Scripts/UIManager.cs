using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the game's user interface elements and interactions.
/// Handles displaying and hiding UI panels, and processing UI-related input.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    /// <summary>
    /// Panel containing game controls information.
    /// </summary>
    public GameObject controlsPanel;
    
    /// <summary>
    /// Timer tracking how long the controls have been displayed.
    /// </summary>
    private float displayTimer;
    
    /// <summary>
    /// Indicates whether the controls panel is currently being displayed.
    /// </summary>
    private bool isDisplayingControls;

    /// <summary>
    /// Initializes UI elements and sets their initial state.
    /// </summary>
    void Start()
    {
        // Initially hide the controls panel
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Processes input and updates UI elements.
    /// Called once per frame.
    /// </summary>
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

    /// <summary>
    /// Shows the controls panel for the specified duration.
    /// </summary>
    public void ShowControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
            isDisplayingControls = true;
        }
    }

    /// <summary>
    /// Hides the controls panel.
    /// </summary>
    private void HideControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
            isDisplayingControls = false;
        }
    }
} 