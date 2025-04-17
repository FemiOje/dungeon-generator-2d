using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonGenerator : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField startPositionInput;
    public TMP_InputField sizeXInput;
    public TMP_InputField sizeYInput;
    public Button startButton;
    public GameObject uiPanel;

    [Header("References")]
    public CameraController cameraController;

    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;

        public bool obligatory;

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

    public Vector2Int size;
    public int startPos = 0;
    public Rule[] rooms;
    public Vector2 offset;
    public GameObject playerPrefab;

    List<Cell> board;
    private bool hasGenerated = false;

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
                Debug.LogError("Size values must be greater than 0!");
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
        }
        else
        {
            Debug.LogError("Invalid input values!");
        }
    }

    void GenerateDungeon()
    {
        bool playerSpawned = false;

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
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " " + i + "-" + j;
                    
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
