using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] walls; // 0 - Up, 1 - Down, 2 - Right, 3 - Left
    public GameObject[] doors; // 0 - Up, 1 - Down, 2 - Right, 3 - Left
    public bool isEndRoom = false;
    private bool hasTriggeredEnd = false;

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