using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enum set for different type types
public enum Tiletype
{
    Open,
    Obstacle,
    Start,
    Goal
}

public class Tile : MonoBehaviour
{
    //initializing variables for tiles and grid collection
    private int value;
    private int rowIndex;
    private int colIndex;
    private SpriteRenderer spriteRenderer;
    public Tiletype tileType;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    // assigns each item in the enum a color in the sprite renderer
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

    //collects values of selected tile
    public void Initialize(int tileValue, int row, int col)
    {
        value = tileValue;
        rowIndex = row;
        colIndex = col;
    }

    //collects row index 
    public int GetRowIndex()
    {
        return rowIndex;
    }

    //collects column index
    private int GetColIndex()
    {
        return colIndex;
    }

    // if no color is assigned, give it a color
    public void SetColor(Color newColor)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = newColor;
    }
}