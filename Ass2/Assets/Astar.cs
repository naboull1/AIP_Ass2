using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : MonoBehaviour
{
    //reference to the GridManager
    public Tile[,] gridReference;

    //delay variable
    public float stepDelay = 0.02f;


    //main function for A* path finding
    public IEnumerator FindShortestPathCoroutine(Vector2Int start, Vector2Int goal, int gridWidth, int gridHeight, System.Action<List<Vector2Int>> onPathFound)
    {
        //open and closed lists
        List<Vector2Int> openSet = new List<Vector2Int> { start };
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        //cost maps
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, int> fScore = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        gScore[start] = 0;
        fScore[start] = ManhattanDistance(start, goal);

        while (openSet.Count > 0)
        {
            //find the node in openSet with the lowest fScore
            Vector2Int current = openSet[0];
            foreach (var tile in openSet)
            {
                if (fScore.ContainsKey(tile) && fScore[tile] < fScore[current])
                    current = tile;
            }

            //if the goal is reached, start reconstructin the path
            if (current == goal)
            {
                List<Vector2Int> finalPath = ReconstructPath(cameFrom, current);
                onPathFound?.Invoke(finalPath);
                yield break;
            }

            //swaps the tiles into the confirmed list
            openSet.Remove(current);
            closedSet.Add(current);

            //changes colors of tiles that A* has checked to grey
            if (gridReference != null)
            {
                Tile tile = gridReference[current.y, current.x];
                if (tile.tileType != Tiletype.Start && tile.tileType != Tiletype.Goal)
                    tile.SetColor(Color.gray);
            }

            yield return new WaitForSeconds(stepDelay);

            //checks neighbours (up, down, left, right)
            foreach (var neighbour in GetNeighbours(current, gridWidth, gridHeight))
            {
                if (closedSet.Contains(neighbour))
                    continue;

                Tile neighbourTile = gridReference[neighbour.y, neighbour.x];

                //skip obstacles
                if (neighbourTile.tileType == Tiletype.Obstacle)
                    continue;

                int tentativeG = gScore[current] + 1;

                if (!openSet.Contains(neighbour))
                    openSet.Add(neighbour);
                else if (tentativeG >= gScore[neighbour])
                    continue;

                cameFrom[neighbour] = current;
                gScore[neighbour] = tentativeG;
                fScore[neighbour] = gScore[neighbour] + ManhattanDistance(neighbour, goal);
            }
        }

        //cant find path
        onPathFound?.Invoke(new List<Vector2Int>());
    }

    //reconstruct the final path
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

    //manhattan Distance = steps ignoring diagonals
    int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    //return valid up/down/left/right neighbours
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
}
