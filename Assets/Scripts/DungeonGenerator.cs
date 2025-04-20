using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Generates a procedural dungeon using a maze generation algorithm.
/// Controls the creation of rooms, placement of walls and doors, and player spawning.
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    [Header("UI References")]
    /// <summary>
    /// Input field for the starting position of the maze generation.
    /// </summary>
    public TMP_InputField startPositionInput;
    
    /// <summary>
    /// Input field for the width of the dungeon.
    /// </summary>
    public TMP_InputField sizeXInput;
    
    /// <summary>
    /// Input field for the height of the dungeon.
    /// </summary>
    public TMP_InputField sizeYInput;
    
    /// <summary>
    /// Button to start the dungeon generation process.
    /// </summary>
    public Button startButton;
    
    /// <summary>
    /// Panel containing the UI elements for dungeon generation.
    /// </summary>
    public GameObject uiPanel;

    [Header("References")]
    /// <summary>
    /// Reference to the camera controller component.
    /// </summary>
    public CameraController cameraController;
    
    /// <summary>
    /// Reference to the UI manager component.
    /// </summary>
    public UIManager uiManager;

    /// <summary>
    /// Represents a single cell in the dungeon grid.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Indicates whether this cell has been visited during maze generation.
        /// </summary>
        public bool visited = false;
        
        /// <summary>
        /// Array indicating which walls are open (true = open, false = closed).
        /// Index 0: Up, 1: Down, 2: Right, 3: Left.
        /// </summary>
        public bool[] status = new bool[4];
    }

    /// <summary>
    /// Defines a rule for room placement in the dungeon.
    /// </summary>
    [System.Serializable]
    public class Rule
    {
        /// <summary>
        /// The room prefab to be placed.
        /// </summary>
        public GameObject room;
        
        /// <summary>
        /// Minimum position where this room can be placed.
        /// </summary>
        public Vector2Int minPosition;
        
        /// <summary>
        /// Maximum position where this room can be placed.
        /// </summary>
        public Vector2Int maxPosition;

        /// <summary>
        /// Whether this room must be placed if the position conditions are met.
        /// </summary>
        public bool obligatory;

        /// <summary>
        /// Calculates the probability of spawning this room at the given position.
        /// </summary>
        /// <param name="x">X coordinate of the position</param>
        /// <param name="y">Y coordinate of the position</param>
        /// <returns>0: Cannot spawn, 1: Can spawn, 2: Must spawn</returns>
        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn

            if (x>= minPosition.x && x<=maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }

            return 0;
        }
    }

    /// <summary>
    /// Size of the dungeon grid (width, height).
    /// </summary>
    public Vector2Int size;
    
    /// <summary>
    /// Starting position for maze generation.
    /// </summary>
    public int startPos = 0;
    
    /// <summary>
    /// Array of room placement rules.
    /// </summary>
    public Rule[] rooms;
    
    /// <summary>
    /// Offset between adjacent rooms.
    /// </summary>
    public Vector2 offset;
    
    /// <summary>
    /// Prefab for the player character.
    /// </summary>
    public GameObject playerPrefab;

    /// <summary>
    /// List of cells in the dungeon grid.
    /// </summary>
    List<Cell> board;
    
    /// <summary>
    /// Indicates whether a dungeon has been generated.
    /// </summary>
    private bool hasGenerated = false;

    /// <summary>
    /// Initializes the dungeon generator and sets up UI elements.
    /// </summary>
    void Start()
    {
        // Add listener to the start button
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }
        // Initialize input fields with default values
        if (sizeXInput != null) sizeXInput.text = size.x.ToString();
        if (sizeYInput != null) sizeYInput.text = size.y.ToString();
        if (startPositionInput != null) startPositionInput.text = startPos.ToString();
    }

    /// <summary>
    /// Handles the start button click event, validating input and starting dungeon generation.
    /// </summary>
    void OnStartButtonClick()
    {
        // Parse user input
        if (int.TryParse(sizeXInput.text, out int x) && 
            int.TryParse(sizeYInput.text, out int y) && 
            int.TryParse(startPositionInput.text, out int start))
        {
            // Validate input
            if (x <= 0 || y <= 0)
            {
                // Debug.LogError("Size values must be greater than 0!");
                return;
            }

            size = new Vector2Int(x, y);
            startPos = Mathf.Clamp(start, 0, x * y - 1);

            // Clear existing dungeon if it exists
            if (hasGenerated)
            {
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
            }

            // Hide UI panel
            if (uiPanel != null) uiPanel.SetActive(false);

            // Generate new dungeon
            MazeGenerator();
            hasGenerated = true;

            // Initialize camera after dungeon generation
            if (cameraController != null)
            {
                // Calculate the center of the dungeon
                Vector3 dungeonCenter = new Vector3(
                    (size.x - 1) * offset.x * 0.5f,
                    -(size.y - 1) * offset.y * 0.5f,
                    0
                );

                // Calculate the size needed to view the entire dungeon
                float maxSize = Mathf.Max(size.x * offset.x, size.y * offset.y);
                maxSize = maxSize * 0.7f;
                
                // Update camera with calculated values
                cameraController.UpdateDungeonBounds(transform, maxSize);
                cameraController.InitializeCamera(dungeonCenter);
            }

            // Show controls after dungeon generation
            if (uiManager != null)
            {
                uiManager.ShowControls();
            }
        }
        else
        {
            // Debug.LogError("Invalid input values!");
        }
    }

    /// <summary>
    /// Generates a maze using a depth-first search algorithm.
    /// </summary>
    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        while (k<1000)
        {
            k++;

            board[currentCell].visited = true;

            if(currentCell == board.Count - 1)
            {
                break;
            }

            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if (newCell > currentCell)
                {
                    //down or right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
            }
        }
        GenerateDungeon();
    }

    /// <summary>
    /// Creates the physical dungeon based on the generated maze.
    /// Places rooms, sets up walls and doors, and spawns the player.
    /// </summary>
    public void GenerateDungeon()
    {
        bool playerSpawned = false;
        int endRoomX = -1;
        int endRoomY = -1;
        float maxDistance = 0f;

        // First pass: Find the furthest room from start
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];
                if (currentCell.visited)
                {
                    // Calculate distance from start position
                    int startX = startPos % size.x;
                    int startY = startPos / size.x;
                    float distance = Mathf.Sqrt(Mathf.Pow(i - startX, 2) + Mathf.Pow(j - startY, 2));
                    
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        endRoomX = i;
                        endRoomY = j;
                    }
                }
            }
        }

        // Debug.Log($"End room calculated at position: ({endRoomX}, {endRoomY}) with distance: {maxDistance}");

        // Second pass: Create rooms and set end room
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];
                if (currentCell.visited)
                {
                    int randomRoom = -1;
                    List<int> availableRooms = new List<int>();

                    for (int k = 0; k < rooms.Length; k++)
                    {
                        int p = rooms[k].ProbabilityOfSpawning(i, j);

                        if(p == 2)
                        {
                            randomRoom = k;
                            break;
                        } else if (p == 1)
                        {
                            availableRooms.Add(k);
                        }
                    }

                    if(randomRoom == -1)
                    {
                        if (availableRooms.Count > 0)
                        {
                            randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                        }
                        else
                        {
                            randomRoom = Random.Range(0, rooms.Length);
                        }
                    }

                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector2(i * offset.x, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.name += " " + i + "-" + j;

                    // Set this room as the end room if it matches the calculated end room position
                    if (i == endRoomX && j == endRoomY)
                    {
                        newRoom.isEndRoom = true;
                        // Debug.Log($"Setting end room at: {newRoom.name}");
                    }
                    newRoom.UpdateRoom(currentCell.status);
                    
                    if (!playerSpawned && playerPrefab != null)
                    {
                        Vector3 playerPosition = new Vector3(i * offset.x, -j * offset.y, 0);
                        Instantiate(playerPrefab, playerPosition, Quaternion.identity);
                        playerSpawned = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks for unvisited neighboring cells in the maze grid.
    /// </summary>
    /// <param name="cell">Index of the current cell</param>
    /// <returns>List of indices of unvisited neighboring cells</returns>
    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[(cell-size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell+1) % size.x != 0 && !board[(cell +1)].visited)
        {
            neighbors.Add((cell +1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell -1));
        }

        return neighbors;
    }
}
