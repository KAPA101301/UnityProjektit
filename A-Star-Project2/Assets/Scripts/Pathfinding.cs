using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pathfinding class implementing A* algorithm to find the shortest path in a grid.
public class Pathfinding
{
    // Cost constants for movement.
    private const int MOVE_STRAIGHT_COST = 10; // Cost for moving straight.
    private const int MOVE_DIAGONAL_COST = 14; // Cost for moving diagonally.

    private Grid<PathNode> grid;     // Grid representation.
    private List<PathNode> openList; // List of nodes to be evaluated.
    private List<PathNode> closedList; // List of nodes already evaluated.

    // Constructor to initialize the grid with specified dimensions.
    public Pathfinding(int width, int height)
    {
        grid = new Grid<PathNode>(
            width,
            height,
            10f,
            Vector3.zero,
            (Grid<PathNode> grid, int x, int y) => new PathNode(grid, x, y)
        );
    }

    // Get the underlying grid.
    public Grid<PathNode> GetGrid()
    {
        return grid;
    }

    // Finds the shortest path from start to end coordinates using A*.
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        // Retrieve start and end nodes from the grid.
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        // Initialize the open and closed lists.
        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        // Reset all nodes' costs and cameFromNode.
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        // Set up the starting node.
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        // Debug visuals for pathfinding steps.
        PathfindingDebugStepVisual.Instance.ClearSnapshots();
        PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, startNode, openList, closedList);

        // Main pathfinding loop.
        while (openList.Count > 0)
        {
            // Get the node with the lowest fCost from the open list.
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                // Reached the destination.
                PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, startNode, openList, closedList);
                PathfindingDebugStepVisual.Instance.TakeSnapshotFinalPath(grid, CalculatePath(endNode));
                return CalculatePath(endNode);
            }

            // Move the current node from open to closed list.
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Evaluate neighboring nodes.
            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;

                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                // Calculate tentative gCost and update node if it's better.
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }

                // Debug snapshot for each step.
                PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, startNode, openList, closedList);
            }
        }

        // No valid path found.
        return null;
    }

    // Gets a list of valid neighboring nodes for the current node.
    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        // Check left neighbors.
        if (currentNode.x - 1 >= 0)
        {
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y)); // Left
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1)); // Left down
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1)); // Left up
        }

        // Check right neighbors.
        if (currentNode.x + 1 < grid.GetWidth())
        {
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y)); // Right
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1)); // Right down
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1)); // Right up
        }

        // Check up and down neighbors.
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1)); // Down
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1)); // Up

        return neighbourList;
    }

    // Retrieves a node from the grid based on its coordinates.
    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    // Calculates the path from the end node by backtracking using cameFromNode.
    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode> { endNode };
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse(); // Reverse to get the correct order from start to end.
        return path;
    }

    // Calculates the distance cost between two nodes using Manhattan distance.
    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    // Finds the node with the lowest fCost in a given list of nodes.
    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}