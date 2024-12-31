using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Visualizes the pathfinding grid by rendering walkable and non-walkable areas as a mesh.
public class PathfindingVisual : MonoBehaviour
{
    private Grid<PathNode> grid; // Reference to the grid used for pathfinding.
    private Mesh mesh; // Mesh used to visually represent the grid.
    private bool updateMesh; // Flag to indicate if the mesh needs updating.

    // Called when the script instance is being loaded.
    private void Awake()
    {
        // Initialize the mesh and assign it to the MeshFilter component.
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Sets the grid and initializes its visual representation.
    public void SetGrid(Grid<PathNode> grid)
    {
        this.grid = grid;
        UpdateVisual(); // Update the visuals to reflect the current state of the grid.

        // Subscribe to the grid's object change event to update visuals when grid data changes.
        grid.OnGridObjectChanged += Grid_OnGridValueChanged;
    }

    // Event handler triggered when a grid object changes.
    private void Grid_OnGridValueChanged(object sender, Grid<PathNode>.OnGridObjectChangedEventArgs e)
    {
        updateMesh = true; // Set the flag to update the mesh in the next frame.
    }

    // Called every frame after all other updates.
    private void LateUpdate()
    {
        // Check if the mesh needs to be updated.
        if (updateMesh)
        {
            updateMesh = false; // Reset the flag.
            UpdateVisual(); // Rebuild the mesh visuals.
        }
    }

    // Updates the visual representation of the grid as a mesh.
    private void UpdateVisual()
    {
        // Prepare empty mesh arrays for vertices, UVs, and triangles.
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        // Loop through all grid positions to build the mesh.
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y; // Calculate the unique index for each grid cell.
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize(); // Define the size of the quad for the grid cell.

                PathNode pathNode = grid.GetGridObject(x, y); // Get the PathNode at the current grid position.

                // Set quad size to zero for walkable nodes (invisible).
                if (pathNode.isWalkable)
                {
                    quadSize = Vector3.zero;
                }

                // Add the quad to the mesh arrays with appropriate properties.
                MeshUtils.AddToMeshArrays(
                    vertices,
                    uv,
                    triangles,
                    index,
                    grid.GetWorldPosition(x, y) + quadSize * 0.5f,
                    0f,
                    quadSize,
                    Vector2.zero,
                    Vector2.zero
                );
            }
        }

        // Assign the mesh arrays to the mesh.
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}


