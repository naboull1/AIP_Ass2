using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Tiletype
{
    Open,
    Obstacle,
    Start,
    Goal
}


public class Tiles : MonoBehaviour
{
    private int value;
    private SpriteRenderer spriteRenderer;
    public Tiletype tileType;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (tileType ==  Tiletype.Open)
        {
            spriteRenderer.color = Color.white;
        }
        if (tileType == Tiletype.Obstacle)
        {
            spriteRenderer.color = Color.red;
        }
        if (tileType == Tiletype.Start)
        {
            spriteRenderer.color = Color.green;
        }
        if (tileType == Tiletype.Goal)
        {
            spriteRenderer.color = Color.cyan;
        }
    }
}