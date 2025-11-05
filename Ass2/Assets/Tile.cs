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



public class Tile : MonoBehaviour
{
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

    public void Initialize(int tileValue, int row, int col)
    {
        value = tileValue;
        rowIndex = row;
        colIndex = col;
    }

    public int GetRowIndex()
    {
        return rowIndex;
    }

    private int GetColIndex()
    {
        return colIndex;
    }
    public void SetColor(Color newColor)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = newColor;
    }


}