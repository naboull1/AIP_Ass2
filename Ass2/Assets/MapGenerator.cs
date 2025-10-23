using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public GameObject tilePrefab;
    public float padding = 0.1f;
    public float tileSize = 1.0f;
    private Tile[,] grid;
    private Tile startTile;
    private Tile goalTile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadCSV("map.csv");
    }

    public void LoadCSV(string csv)
    {
        string csvFilePath = Path.Combine(Application.dataPath, csv);  //../Assets/maps.csv
        List<string> lines = new List<string>();
        using (StreamReader reader = new StreamReader(csvFilePath))
        {
            while (!reader.EndOfStream)
            {
                lines.Add(reader.ReadLine());
            }
        }

        int numRows = lines.Count;
        int numCols = lines[0].Split(',').Length;

        Tiletype[,] tileMap = new Tiletype[numRows, numCols];
        int start_i = 0;
        int start_j = 0;
        int goal_i = 0;
        int goal_j = 0;



        for (int i = 0; i < numRows; i++)
        {
            string[] values = lines[i].Split(",");
            for (int j = 0; j < numCols; j++)
            {
                switch (int.Parse(values[j].Trim()))
                {
                    case 0:
                        tileMap[i, j] = Tiletype.Open;
                        break;
                    case 1:
                        tileMap[i, j] = Tiletype.Obstacle;
                        break;
                    case 2:
                        tileMap[i, j] = Tiletype.Start;
                        start_i = i;
                        start_j = j;
                        break;
                    case 3:
                        tileMap[i, j] = Tiletype.Goal;
                        goal_i = i;
                        goal_j = j;
                        break;
                }
            }
        }





        grid = new Tile[numRows, numCols];
        for (int y = numRows - 1; y >= 0; y--)
        {
            for (int x = 0; x < numCols; x++)
            {
                Vector3 position = new Vector3(x * (tileSize + padding), (numRows - y - 1) * (tileSize + padding), 0.0f);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                Tile tileScript = tile.GetComponent<Tile>();

                if (tileScript != null)
                {
                    tileScript.tileType = tileMap[y, x];
                    tileScript.Initialize(1, y, x);
                    grid[y, x] = tileScript;
                    
                }
            }
        }
        //startTile = grid[start_i, start_j];
    }
}
