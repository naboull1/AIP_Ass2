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


    public List<Tile> FindShortestPath()
    {
        Dictionary<Tile, float> gScore = new Dictionary<Tile, float>();
        Dictionary<Tile, float> hScore = new Dictionary<Tile, float>();
        Dictionary<Tile, float> cameFrom = new Dictionary<Tile, float>();


        //initialize the grid

        //loop
        //once you explore your neighbours, calculate g and h scores
        // visit the neighbour with the lowst f score (g+h)
        //if you reached the goal, reconstruct the path using the camefrom dictionary 
    }

    public float ManhattanDistance()
    {
        //Manhattan distance heuristic

    }


}
