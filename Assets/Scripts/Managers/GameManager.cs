using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Manages the core game functionality including game state, UI interactions, and game flow control.
/// Implements the Singleton pattern to ensure only one instance exists throughout the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the GameManager. Accessible from anywhere in the game.
    /// </summary>
    public static GameManager Instance { get; private set; }

    [Header("UI Elements")]
    /// <summary>
    /// Panel displayed when the game ends.
    /// </summary>
    public GameObject endGamePanel;
    
    /// <summary>
    /// Text component displaying the end game message.
    /// </summary>
    public TextMeshProUGUI endGameText;
    
    /// <summary>
    /// Button to restart the game.
    /// </summary>
    public Button restartButton;
    
    /// <summary>
    /// Button to quit the game.
    /// </summary>
    public Button quitButton;
    
    /// <summary>
    /// Particle effect prefab to display when player reaches the end.
    /// </summary>
    public GameObject particleEffect;

    [Header("References")]
    /// <summary>
    /// Reference to the dungeon generator component.
    /// </summary>
    public DungeonGenerator dungeonGenerator;
    
    /// <summary>
    /// Reference to the UI manager component.
    /// </summary>
    public UIManager uiManager;

    /// <summary>
    /// Initializes the singleton instance and ensures only one GameManager exists.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sets up initial game state and UI elements.
    /// </summary>
    private void Start()
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    /// <summary>
    /// Handles the event when the player reaches the end of the dungeon.
    /// Displays particle effects, shows end game UI, and pauses the game.
    /// </summary>
    public void OnPlayerReachedEnd()
    {
        // Show particle effect
        if (particleEffect != null)
        {
            Instantiate(particleEffect, GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
        }

        // Show end game UI
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
        }

        // Show congratulatory message
        if (endGameText != null)
        {
            endGameText.text = "Congratulations!\nYou've reached the end of the dungeon!";
        }

        // Pause the game
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Restarts the game by resetting time scale, hiding UI elements, and regenerating the dungeon.
    /// </summary>
    public void RestartGame()
    {
        // Reset time scale
        Time.timeScale = 1f;

        // Hide end game panel
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }

        // Show the initial UI panel
        if (dungeonGenerator != null && dungeonGenerator.uiPanel != null)
        {
            dungeonGenerator.uiPanel.SetActive(true);
        }

        // Clear existing dungeon
        if (dungeonGenerator != null)
        {
            foreach (Transform child in dungeonGenerator.transform)
            {
                Destroy(child.gameObject);
            }
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
    }

    /// <summary>
    /// Quits the game. In editor mode, stops play mode. In build, exits the application.
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 