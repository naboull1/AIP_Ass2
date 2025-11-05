using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    //Initialization of variables for the grid
    public GameObject tilePrefab;
    public Astar astar;
    public int rows = 50;
    public int cols = 50;
    private Tile[,] grid;
    // Initializing the 2 main important tiles, start and goal tiles
    private Tile startTile;
    private Tile goalTile;
    private float TempRandom;
    bool executeOnce = false;
    public Text uiText; // Drag your Text component here in the Inspector
    public float displayDuration = 3f;


    void Start()
    {
        InitializeGrid();
        StartCoroutine(RunContinuousPathfinding());
        uiText.gameObject.SetActive(false); // Hide text initially
    }

    IEnumerator RunContinuousPathfinding()
    {
        while (true)
        {
            if (astar != null && startTile != null && goalTile != null)
            {
                astar.gridReference = grid;

                Vector2Int startPos = new Vector2Int(GetColIndex(startTile), startTile.GetRowIndex());
                Vector2Int goalPos = new Vector2Int(GetColIndex(goalTile), goalTile.GetRowIndex());

                // Run pathfinding and wait until it’s done
                bool pathComplete = false;
                StartCoroutine(astar.FindShortestPathCoroutine(startPos, goalPos, cols, rows, (path) =>
                {
                    ShowPath(path);
                    pathComplete = true;
                    ShowText("Got your NOSE!");
                }));

                // Wait until current path finishes
                while (!pathComplete)
                    yield return null;

                // ⏳ Pause a few seconds before restarting

                yield return new WaitForSeconds(2f);

                // Make the goal the new start
                startTile.tileType = Tiletype.Start;
                startTile.SetColor(Color.green);

                goalTile.tileType = Tiletype.Open;
                goalTile.SetColor(Color.white);

                startTile = goalTile;

                // 🆕 Randomize obstacles again
                RegenerateObstacles();
                HideText();

                // Pick a new random goal tile that isn’t an obstacle or the same as start
                Tile newGoal = null;
                while (newGoal == null || newGoal == startTile || newGoal.tileType == Tiletype.Obstacle)
                {
                    int newRow = Random.Range(0, rows);
                    int newCol = Random.Range(0, cols);
                    newGoal = grid[newRow, newCol];
                }

                newGoal.tileType = Tiletype.Goal;
                newGoal.SetColor(Color.cyan);
                goalTile = newGoal;

                // 🧹 Clear colors except Start/Goal/Obstacles before next run
                ResetOpenTiles();
            }

            yield return null;
        }
    }

    void RegenerateObstacles()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Tile tile = grid[row, col];

                if (tile == startTile || tile == goalTile)
                    continue; // don’t touch start/goal

                float randomVal = Random.value;

                // ~10% chance of obstacle
                if (randomVal < 0.1f)
                {
                    tile.tileType = Tiletype.Obstacle;
                    tile.SetColor(Color.red);
                }
                else
                {
                    tile.tileType = Tiletype.Open;
                    tile.SetColor(Color.white);
                }
            }
        }
    }

    void ResetOpenTiles()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Tile tile = grid[row, col];
                if (tile.tileType == Tiletype.Open)
                {
                    tile.SetColor(Color.white);
                }
            }
        }
    }

    void InitializeGrid()
    {
        // setup grid and assign random value for the goal tile
        grid = new Tile[rows, cols]; 
        int goalRow = Random.Range(0, rows);
        int goalCol = Random.Range(0, cols);

        //ensures the goal tile never spaws where out start point is
        if (goalRow == 0 && goalCol == 0)
        {
            goalRow = Random.Range(0, rows);
            goalCol = Random.Range(0, cols);
        }


        //Loop for creating start tile then with each loop places a random obstacle or open tile with one random point for goal tile
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject tileObj = Instantiate(tilePrefab, new Vector3(col, -row, 0), Quaternion.identity);
                Tile tile = tileObj.GetComponent<Tile>();
                tile.Initialize(0, row, col);

                float TempRandom = Random.value;
                print($"random value is: {TempRandom}");

                // random tile placement for obstacle and open tiles
                if (TempRandom < 0.1f)
                    tile.tileType = Tiletype.Obstacle;
                else
                    tile.tileType = Tiletype.Open;

                // drops first tile at 0
                if (row == 0 && col == 0)
                {
                    tile.tileType = Tiletype.Start;
                    startTile = tile;
                }

                // drops goal tile at random location
                if (row == goalRow && col == goalCol)
                {
                    tile.tileType = Tiletype.Goal;
                    goalTile = tile;
                }
                grid[row, col] = tile;
            }
        }
    }

    void ShowPath(List<Vector2Int> path)
    {
        foreach (Vector2Int pos in path)
        {
            Tile tile = grid[pos.y, pos.x]; // assuming grid[row, col]
            tile.SetColor(Color.yellow);
        }
    }



    // Manhattan distance function
    int ManhattanDistance(Tile a, Tile b)
    {
        return Mathf.Abs(a.GetRowIndex() - b.GetRowIndex()) + Mathf.Abs(GetColIndex(a) - GetColIndex(b));
    }


    int GetColIndex(Tile tile)
    {
        var colField = typeof(Tile).GetField("colIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (int)colField.GetValue(tile);
    }

    public void ShowText(string message)
    {
        uiText.text = message; // Set the text content
        uiText.gameObject.SetActive(true); // Show the text
        Invoke("HideText", displayDuration); // Hide after duration
    }

    void HideText()
    {
        uiText.gameObject.SetActive(false); // Hide the text
    }
}
