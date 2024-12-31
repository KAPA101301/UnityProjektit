using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;


// Class for testing the pathfinding system with visual debugging.
public class Testing : MonoBehaviour
{
    [SerializeField] private PathfindingDebugStepVisual pathfindingDebugStepVisual; // Visualizes step-by-step pathfinding process.
    [SerializeField] private PathfindingVisual pathfindingVisual; // Visualizes the grid and pathfinding output.

    private Pathfinding pathfinding; // Instance of the Pathfinding class.

    // Initialization method, called at the start of the script lifecycle.
    private void Start()
    {
        // Create a new Pathfinding instance with a grid of size 20x10.
        pathfinding = new Pathfinding(10, 10);

        // Setup the debugging visuals to use the same grid.
        pathfindingDebugStepVisual.Setup(pathfinding.GetGrid());

        // Setup the grid visual representation.
        pathfindingVisual.SetGrid(pathfinding.GetGrid());
    }

    // Update method, called once per frame.
    private void Update()
    {
        // Handle left mouse button clicks to find and draw a path.
        if (Input.GetMouseButtonDown(0))
        {
            // Get the mouse position in world coordinates.
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();

            // Convert the mouse position to grid coordinates.
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

            // Find a path from (0,0) to the clicked position.
            List<PathNode> path = pathfinding.FindPath(0, 0, x, y);

            // If a valid path is found, draw the path using green lines.
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(
                        new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f,
                        new Vector3(path[i + 1].x, path[i + 1].y) * 10f + Vector3.one * 5f,
                        Color.green
                    );
                }
            }
        }

        // Handle right mouse button clicks to toggle the walkability of a node.
        if (Input.GetMouseButtonDown(1))
        {
            // Get the mouse position in world coordinates.
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();

            // Convert the mouse position to grid coordinates.
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

            // Toggle the walkability of the clicked node.
            pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
        }
    }
}
