using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class representing a single node in the grid for pathfinding.
public class PathNode
{
    private Grid<PathNode> grid; // Reference to the grid this node belongs to.
    public int x; // X-coordinate of the node.
    public int y; // Y-coordinate of the node.

    // Pathfinding costs:
    public int gCost; // Cost from the start node to this node.
    public int hCost; // Estimated cost from this node to the end node (heuristic).
    public int fCost; // Total cost (gCost + hCost).

    public bool isWalkable; // Indicates whether this node is walkable.
    public PathNode cameFromNode; // Reference to the previous node in the path.

    // Constructor to initialize the node with its grid reference and coordinates.
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true; // By default, all nodes are walkable.
    }

    // Calculates the total cost (fCost) by summing gCost and hCost.
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    // Sets whether the node is walkable and notifies the grid of the change.
    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
        grid.TriggerGridObjectChanged(x, y); // Notify the grid to update.
    }

    // Overrides the ToString method to return the node's coordinates as a string.
    public override string ToString()
    {
        return x + "," + y; // Returns the format "x,y".
    }
}
