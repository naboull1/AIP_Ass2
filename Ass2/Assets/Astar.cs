using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Astar : MonoBehaviour
{
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}



    public List<Vector2Int> FindShortestPath(Vector2Int start, Vector2Int goal, int gridWidth, int gridHeight)
    {
        // The open and closed sets
        List<Vector2Int> openSet = new List<Vector2Int> { start };
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        // Store cost so far and estimated total cost
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, int> hScore = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        gScore[start] = 0;
        hScore[start] = ManhattanDistance(start, goal);

        while (openSet.Count > 0)
        {
            // Find tile in open set with lowest fScore
            Vector2Int current = openSet[0];
            foreach (var tile in openSet)
            {
                if (hScore.ContainsKey(tile) && hScore[tile] < hScore[current])
                    current = tile;
            }

            // If reached the goal, reconstruct and return path
            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            // Explore neighbours (up, down, left, right)
            foreach (var neighbour in GetNeighbours(current, gridWidth, gridHeight))
            {
                if (closedSet.Contains(neighbour))
                    continue;

                int tentativeG = gScore[current] + 1; // cost of moving to neighbour

                if (!openSet.Contains(neighbour))
                    openSet.Add(neighbour);
                else if (tentativeG >= gScore[neighbour])
                    continue;

                // Record best path
                cameFrom[neighbour] = current;
                gScore[neighbour] = tentativeG;
                hScore[neighbour] = gScore[neighbour] + ManhattanDistance(neighbour, goal);
            }
        }

        // No path found
        return new List<Vector2Int>();
    }

    // Reconstruct path backwards
    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> totalPath = new List<Vector2Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    // Manhattan distance
    int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Get neighbours (up/down/left/right)
    List<Vector2Int> GetNeighbours(Vector2Int tile, int width, int height)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        Vector2Int[] directions = {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbour = tile + dir;
            if (neighbour.x >= 0 && neighbour.x < width && neighbour.y >= 0 && neighbour.y < height)
                neighbours.Add(neighbour);
        }

        return neighbours;
    }






        //initialize the grid

        //loop
        //once you explore your neighbours, calculate g and h scores
        // visit the neighbour with the lowst f score (g+h)
        //if you reached the goal, reconstruct the path using the camefrom dictionary 
    



}
