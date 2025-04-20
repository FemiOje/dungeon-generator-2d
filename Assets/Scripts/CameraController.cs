using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the main camera's behavior, including following the player, zooming, and switching between views.
/// Provides smooth camera movement and automatic positioning based on dungeon size.
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// Camera settings
    /// </summary>
    [Header("Camera Settings")]
    /// <summary>
    /// Speed at which the camera smoothly moves to its target position.
    /// </summary>
    public float smoothSpeed = 5f;
    
    /// <summary>
    /// Speed at which the camera zooms in and out.
    /// </summary>
    public float zoomSpeed = 2f;

    /// <summary>
    /// Key with which to toggle camera views
    /// </summary>
    public KeyCode toggleKey = KeyCode.Tab;
    
    [Header("References")]
    /// <summary>
    /// Reference to the player's transform for following.
    /// </summary>
    private Transform player;

    /// <summary>
    /// Center transform of the dungeon
    /// </summary>
    public Transform dungeonCenter;

    /// <summary>
    /// Size of dungeon(default 10.0f)
    /// </summary>
    public float dungeonSize = 10f;
    
    [Header("Camera Modes")]
    /// <summary>
    /// Orthographic size when viewing the entire dungeon.
    /// </summary>
    public float fullViewSize = 10f;

    /// <summary>
    /// Orthographic size when following the player (default 5.0f)
    /// </summary>
    public float playerViewSize = 5f;
    
    /// <summary>
    /// Sets whether the camera is following the player
    /// </summary>
    private bool isFollowingPlayer = false;

    /// <summary>
    /// Position to which the camera should move
    /// </summary>
    private Vector3 targetPosition;

    /// <summary>
    /// Adjusts to enable player see appropriate elements according to camera view
    /// </summary>
    private float targetSize;

    /// <summary>
    /// Reference to the main camera
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// Sets whether the camera is initialized
    /// </summary>
    private bool isInitialized = false;
    
    /// <summary>
    /// Initializes the camera component and sets default values.
    /// </summary>
    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("CameraController requires a Camera component!");
            enabled = false;
            return;
        }
        
        // Set initial camera position and size without player reference
        targetPosition = new Vector3(0, 0, transform.position.z);
        // Start with a larger initial size to prevent the view being too small
        fullViewSize = 15f;
        mainCamera.orthographicSize = fullViewSize;
        transform.position = targetPosition;
    }

    /// <summary>
    /// Initializes camera after dungeon generation
    /// </summary>
    /// <param name="centerPosition">Vector3 that represents the center point of the dungeon</param>
    public void InitializeCamera(Vector3 centerPosition)
    {
        player = FindObjectOfType<PlayerController>()?.transform;
        if (player == null)
        {
            Debug.LogError("No PlayerController found in the scene!");
            return;
        }

        isFollowingPlayer = false;
        
        // Set initial position to the provided center
        targetPosition = new Vector3(centerPosition.x, centerPosition.y, transform.position.z);
        
        // Ensure the camera size is large enough to show the entire dungeon
        targetSize = fullViewSize;
        mainCamera.orthographicSize = fullViewSize;
        
        // Immediately position camera at the center
        transform.position = targetPosition;
        isInitialized = true;
    }

    /// <summary>
    /// Updates camera position and size based on current mode and player position.
    /// Called once per frame.
    /// </summary>
    void Update()
    {
        if (!isInitialized) return;

        // Toggle camera mode on key press
        if (Input.GetKeyDown(toggleKey))
        {
            isFollowingPlayer = !isFollowingPlayer;
        }
        
        // Update target position and size based on current mode
        if (isFollowingPlayer && player != null)
        {
            targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
            targetSize = playerViewSize;
        }
        else
        {
            // Use the last known center position when not following player
            targetSize = fullViewSize;
        }
        
        // Smoothly move camera to target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        
        // Smoothly adjust camera size
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetSize, smoothSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Toggles between follow player view and overhead view
    /// </summary>
    /// <param name="followPlayer">Sets whether the camera should follow the player</param>
    public void SetCameraMode(bool followPlayer)
    {
        if (!isInitialized) return;
        isFollowingPlayer = followPlayer;
    }
    
    /// <summary>
    /// Updates dungeon center and size
    /// </summary>
    /// <param name="center">Dungeon center transform</param>
    /// <param name="size">Dungeon size</param>
    public void UpdateDungeonBounds(Transform center, float size)
    {
        dungeonCenter = center;
        dungeonSize = size;
        // Ensure the full view size is large enough to show the entire dungeon
        fullViewSize = size;
        // Update current target size if we're in full view mode
        if (!isFollowingPlayer)
        {
            targetSize = fullViewSize;
        }
    }
}
