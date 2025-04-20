using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the player character's movement and appearance.
/// Handles input processing and physics-based movement.
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Speed at which the player moves.
    /// </summary>
    public float moveSpeed = 5f;
    
    /// <summary>
    /// Reference to the player's Rigidbody2D component for physics-based movement.
    /// </summary>
    private Rigidbody2D rb;
    
    /// <summary>
    /// Vector storing the player's movement direction.
    /// </summary>
    private Vector2 movement;
    
    /// <summary>
    /// Reference to the player's SpriteRenderer component for visual updates.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Initializes component references.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Processes player input and updates visual appearance.
    /// Called once per frame.
    /// </summary>
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Flip sprite upside down when moving up, normal otherwise
        if (movement.y > 0)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
    }

    /// <summary>
    /// Applies movement to the player's Rigidbody2D.
    /// Called at a fixed time interval for consistent physics.
    /// </summary>
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
