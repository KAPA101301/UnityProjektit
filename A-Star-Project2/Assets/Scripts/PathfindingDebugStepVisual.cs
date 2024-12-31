using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

// Visualizes each step of the pathfinding process for debugging purposes.
public class PathfindingDebugStepVisual : MonoBehaviour
{
    public static PathfindingDebugStepVisual Instance { get; private set; } // Singleton instance.

    [SerializeField] private Transform pfPathfindingDebugStepVisualNode; // Prefab for visualizing nodes.
    private List<Transform> visualNodeList; // List of all visual nodes.
    private List<GridSnapshotAction> gridSnapshotActionList; // List of actions to visualize grid snapshots.
    private bool autoShowSnapshots; // Whether snapshots should be automatically displayed.
    private float autoShowSnapshotsTimer; // Timer for auto-showing snapshots.
    private Transform[,] visualNodeArray; // 2D array of visual node transforms.

    // Initialize the singleton and lists.
    private void Awake()
    {
        Instance = this;
        visualNodeList = new List<Transform>();
        gridSnapshotActionList = new List<GridSnapshotAction>();
    }

    // Sets up the visual grid with the given pathfinding grid.
    public void Setup(Grid<PathNode> grid)
    {
        visualNodeArray = new Transform[grid.GetWidth(), grid.GetHeight()];

        // Create a visual node for each grid cell.
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Vector3 gridPosition = new Vector3(x, y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * 0.5f;
                Transform visualNode = CreateVisualNode(gridPosition);
                visualNodeArray[x, y] = visualNode;
                visualNodeList.Add(visualNode);
            }
        }

        HideNodeVisuals(); // Initially hide all visual nodes.
    }

    private bool isAnimatingPath; // To track if the animation is in progress

    // Update the visual grid based on input.
    private void Update()
    {
        // Show the next snapshot when the space key is pressed.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextSnapshot();
        }

        // Start auto-showing snapshots when the return key is pressed.
        if (Input.GetKeyDown(KeyCode.Return))
        {
            autoShowSnapshots = true;
        }

        // Auto-show snapshots at intervals.
        if (autoShowSnapshots)
        {
            float autoShowSnapshotsTimerMax = 0.05f;
            autoShowSnapshotsTimer -= Time.deltaTime;
            if (autoShowSnapshotsTimer <= 0f)
            {
                autoShowSnapshotsTimer += autoShowSnapshotsTimerMax;
                ShowNextSnapshot();
                if (gridSnapshotActionList.Count == 0)
                {
                    autoShowSnapshots = false; // Stop when no more snapshots are available.
                }
            }
        }
    }

    // Show the next grid snapshot if available.
    private void ShowNextSnapshot()
    {
        if (gridSnapshotActionList.Count > 0)
        {
            GridSnapshotAction gridSnapshotAction = gridSnapshotActionList[0];
            gridSnapshotActionList.RemoveAt(0);
            gridSnapshotAction.TriggerAction();
        }
    }

    // Clears all stored grid snapshots.
    public void ClearSnapshots()
    {
        gridSnapshotActionList.Clear();
    }

    // Takes a snapshot of the grid, highlighting the current node, open list, and closed list.
    public void TakeSnapshot(Grid<PathNode> grid, PathNode current, List<PathNode> openList, List<PathNode> closedList)
    {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        gridSnapshotAction.AddAction(HideNodeVisuals);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);

                // Capture node properties and position.
                int gCost = pathNode.gCost;
                int hCost = pathNode.hCost;
                int fCost = pathNode.fCost;
                bool isCurrent = pathNode == current;
                bool isInOpenList = openList.Contains(pathNode);
                bool isInClosedList = closedList.Contains(pathNode);
                int tmpX = x;
                int tmpY = y;

                // Add visualization actions for each node.
                gridSnapshotAction.AddAction(() =>
                {
                    Transform visualNode = visualNodeArray[tmpX, tmpY];
                    SetupVisualNode(visualNode, gCost, hCost, fCost);

                    // Set the node's background color based on its state.
                    Color backgroundColor = UtilsClass.GetColorFromString("636363");
                    if (isInClosedList) backgroundColor = new Color(1, 0, 0); // Closed list: Red
                    if (isInOpenList) backgroundColor = UtilsClass.GetColorFromString("009AFF"); // Open list: Blue
                    if (isCurrent) backgroundColor = new Color(0, 1, 0); // Current node: Green

                    visualNode.Find("sprite").GetComponent<SpriteRenderer>().color = backgroundColor;
                });
            }
        }

        gridSnapshotActionList.Add(gridSnapshotAction);
    }

    // Takes a snapshot of the final path, highlighting it in green.
    public void TakeSnapshotFinalPath(Grid<PathNode> grid, List<PathNode> path)
    {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        gridSnapshotAction.AddAction(HideNodeVisuals);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);

                int gCost = pathNode.gCost;
                int hCost = pathNode.hCost;
                int fCost = pathNode.fCost;
                bool isInPath = path.Contains(pathNode);
                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() =>
                {
                    Transform visualNode = visualNodeArray[tmpX, tmpY];
                    SetupVisualNode(visualNode, gCost, hCost, fCost);

                    // Set background color based on path inclusion.
                    Color backgroundColor = isInPath ? new Color(0, 1, 0) : UtilsClass.GetColorFromString("636363");
                    visualNode.Find("sprite").GetComponent<SpriteRenderer>().color = backgroundColor;
                });
            }
        }

        gridSnapshotActionList.Add(gridSnapshotAction);
    }

    // Hides all node visuals by resetting their values.
    private void HideNodeVisuals()
    {
        foreach (Transform visualNodeTransform in visualNodeList)
        {
            SetupVisualNode(visualNodeTransform, 9999, 9999, 9999);
        }
    }

    // Creates a visual node at the specified position.
    private Transform CreateVisualNode(Vector3 position)
    {
        return Instantiate(pfPathfindingDebugStepVisualNode, position, Quaternion.identity);
    }

    // Configures a visual node's text to display the given cost values.
    private void SetupVisualNode(Transform visualNodeTransform, int gCost, int hCost, int fCost)
    {
        if (fCost < 1000)
        {
            visualNodeTransform.Find("gCostText").GetComponent<TextMeshPro>().SetText(gCost.ToString());
            visualNodeTransform.Find("hCostText").GetComponent<TextMeshPro>().SetText(hCost.ToString());
            visualNodeTransform.Find("fCostText").GetComponent<TextMeshPro>().SetText(fCost.ToString());
        }
        else
        {
            visualNodeTransform.Find("gCostText").GetComponent<TextMeshPro>().SetText("");
            visualNodeTransform.Find("hCostText").GetComponent<TextMeshPro>().SetText("");
            visualNodeTransform.Find("fCostText").GetComponent<TextMeshPro>().SetText("");
        }
    }

    // Represents an action to visualize a grid snapshot.
    private class GridSnapshotAction
    {
        private Action action;

        public GridSnapshotAction()
        {
            action = () => { };
        }

        public void AddAction(Action action)
        {
            this.action += action;
        }

        public void TriggerAction()
        {
            action();
        }
    }
}