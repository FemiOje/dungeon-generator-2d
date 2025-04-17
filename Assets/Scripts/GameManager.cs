using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject endGamePanel;
    public TextMeshProUGUI endGameText;
    public Button restartButton;
    public GameObject particleEffect;

    [Header("References")]
    public DungeonGenerator dungeonGenerator;
    public UIManager uiManager;

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

    void Start()
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

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

        // Reset player position if it exists
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
    }
} 