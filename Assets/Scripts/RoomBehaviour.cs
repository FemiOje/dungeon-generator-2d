using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the behavior of individual rooms in the dungeon, including walls, doors, and end room functionality.
/// Manages the visual representation of rooms and handles player interactions with room elements.
/// </summary>
public class RoomBehaviour : MonoBehaviour
{
    /// <summary>
    /// Array of wall GameObjects. Index 0: Up, 1: Down, 2: Right, 3: Left.
    /// </summary>
    public GameObject[] walls;
    
    /// <summary>
    /// Array of door GameObjects. Index 0: Up, 1: Down, 2: Right, 3: Left.
    /// </summary>
    public GameObject[] doors;
    
    /// <summary>
    /// Indicates whether this room is the end room of the dungeon.
    /// </summary>
    public bool isEndRoom = false;
    
    /// <summary>
    /// Tracks whether the end room has already triggered the end game event.
    /// </summary>
    private bool hasTriggeredEnd = false;

    /// <summary>
    /// Updates the room's walls and doors based on the provided status array.
    /// </summary>
    /// <param name="status">Boolean array indicating which walls should be replaced with doors (true = door, false = wall)</param>
    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            // Activate doors and deactivate walls based on the status array
            if (i < doors.Length && i < walls.Length)
            {
                doors[i].SetActive(status[i]);
                walls[i].SetActive(!status[i]);

                // Ensure door colliders are enabled when the door is active
                Collider2D doorCollider = doors[i].GetComponent<Collider2D>();
                if (doorCollider != null)
                {
                    doorCollider.enabled = status[i];
                }
            }
        }
    }

    /// <summary>
    /// Handles trigger events when the player enters the room.
    /// If this is the end room and the player hasn't triggered the end game yet, it will trigger the end game event.
    /// </summary>
    /// <param name="collision">The collider that entered the trigger</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Wall trigger entered.");
        if (isEndRoom && !hasTriggeredEnd && collision.CompareTag("Player"))
        {
            hasTriggeredEnd = true;
            Debug.Log("Player reached end room!");
            
            // Trigger end game event
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerReachedEnd();
            }
            else
            {
                Debug.LogError("GameManager instance not found!");
            }
        }
    }
}