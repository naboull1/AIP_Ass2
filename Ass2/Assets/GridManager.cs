using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int rows = 5;
    public int cols = 5;
    private Tile[,] grid;

    private Tile startTile;
    private Tile goalTile;

    void Start()
    {
        InitializeGrid();
        FindShortestPath();
    }

    void InitializeGrid()
    {
        grid = new Tile[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject tileObj = Instantiate(tilePrefab, new Vector3(col, -row, 0), Quaternion.identity);
                Tile tile = tileObj.GetComponent<Tile>();
                tile.Initialize(0, row, col);

                // Example random assignment of tile type
                if (Random.value < 0.1f)
                    tile.tileType = Tiletype.Obstacle;
                else
                    tile.tileType = Tiletype.Open;

                if (row == 0 && col == 0)
                {
                    tile.tileType = Tiletype.Start;
                    startTile = tile;
                }
                if (row == rows - 1 && col == cols - 1)
                {
                    tile.tileType = Tiletype.Goal;
                    goalTile = tile;
                }

                grid[row, col] = tile;
            }
        }
    }

    // Manhattan Distance
    int ManhattanDistance(Tile a, Tile b)
    {
        return Mathf.Abs(a.GetRowIndex() - b.GetRowIndex()) + Mathf.Abs(GetColIndex(a) - GetColIndex(b));
    }

    int GetColIndex(Tile tile)
    {
        // helper since GetColIndex() is private in Tile.cs
        var colField = typeof(Tile).GetField("colIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (int)colField.GetValue(tile);
    }

    void FindShortestPath()
    {
        // Here you’ll use the same logic as the A* example I showed earlier,
        // except you’ll use your Tile objects instead of Vector2Int.
        // You’ll loop neighbours, calculate g/h/f, and reconstruct the path.

        Debug.Log("Finding shortest path...");
        // TODO: implement A* here or call a helper class that does it
    }
}
