using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

// Generic grid class for managing a 2D grid of objects.
public class Grid<TGridObject>
{
    // Event triggered when a grid object changes.
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;

    // Event argument class for grid object change events.
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x; // The x-coordinate of the grid object.
        public int y; // The y-coordinate of the grid object.
    }

    private int width;               // Width of the grid.
    private int height;              // Height of the grid.
    private float cellSize;          // Size of each cell in the grid.
    private Vector3 originPosition;  // The world position of the grid's origin.
    private TGridObject[,] gridArray; // 2D array storing grid objects.

    // Constructor for initializing the grid.
    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        // Create the grid array and populate it using the provided function.
        gridArray = new TGridObject[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        // Optional debug visualization of the grid.
        bool showDebug = true;
        if (showDebug)
        {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            // Create visual markers for each grid cell.
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, 30, Color.grey, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            // Draw the border lines of the grid.
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            // Update the debug text when a grid object changes.
            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
            {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y].ToString();
            };
        }
    }

    // Get the width of the grid.
    public int GetWidth()
    {
        return width;
    }

    // Get the height of the grid.
    public int GetHeight()
    {
        return height;
    }

    // Get the size of each cell.
    public float GetCellSize()
    {
        return cellSize;
    }

    // Convert grid coordinates to world position.
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    // Convert world position to grid coordinates.
    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    // Set the object at a specific grid coordinate.
    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }
    }

    // Trigger the grid object change event manually.
    public void TriggerGridObjectChanged(int x, int y)
    {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    // Set the object at a grid position determined by world coordinates.
    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    // Get the object at a specific grid coordinate.
    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject); // Return the default value for the type.
        }
    }

    // Get the object at a grid position determined by world coordinates.
    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }
}