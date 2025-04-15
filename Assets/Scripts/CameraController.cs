using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float smoothSpeed = 5f;
    public float zoomSpeed = 2f;
    public KeyCode toggleKey = KeyCode.Tab;
    
    [Header("References")]
    private Transform player;
    public Transform dungeonCenter;
    public float dungeonSize = 10f;
    
    [Header("Camera Modes")]
    public float fullViewSize = 10f;
    public float playerViewSize = 5f;    
    private bool isFollowingPlayer = false;
    private Vector3 targetPosition;
    private float targetSize;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("CameraController requires a Camera component!");
            enabled = false;
            return;
        }

        player = FindObjectOfType<PlayerController>().transform;
        isFollowingPlayer = false;
        
        // Ensure we start with a proper view of the entire scene
        if (dungeonCenter != null)
        {
            targetPosition = new Vector3(dungeonCenter.position.x, dungeonCenter.position.y, transform.position.z);
        }
        else
        {
            targetPosition = new Vector3(0, 0, transform.position.z);
        }
        
        // Dynamically calculate full view size based on dungeon size
        fullViewSize = dungeonSize * 0.5f;
        targetSize = fullViewSize;
        mainCamera.orthographicSize = fullViewSize;
        
        // Ensure camera is positioned correctly from the start
        transform.position = targetPosition;
    }

    void Update()
    {
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
            if (dungeonCenter != null)
            {
                targetPosition = new Vector3(dungeonCenter.position.x, dungeonCenter.position.y, transform.position.z);
            }
            else
            {
                // If no dungeon center is set, position camera at origin
                targetPosition = new Vector3(0, 0, transform.position.z);
            }
            targetSize = fullViewSize;
        }
        
        // Smoothly move camera to target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        
        // Smoothly adjust camera size (orthographic size for 2D)
        if (mainCamera.orthographic)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetSize, smoothSpeed * Time.deltaTime);
        }
    }
    
    // Public method to manually set camera mode
    public void SetCameraMode(bool followPlayer)
    {
        isFollowingPlayer = followPlayer;
    }
    
    // Public method to update dungeon center and size
    public void UpdateDungeonBounds(Transform center, float size)
    {
        dungeonCenter = center;
        dungeonSize = size;
        fullViewSize = size * 0.5f; // Adjust full view size based on dungeon size
    }
}
