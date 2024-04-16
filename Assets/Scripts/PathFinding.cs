using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PathFinding : MonoBehaviour
{

    public static PathFinding Instance {get; private set;}
    private const int moveStraightCost = 10;
    private const int moveDiagonalCost = 14;

    [SerializeField] private Transform gridDebugObjectPrefab;

    [SerializeField] private LayerMask obstacleLayerMask;
    private int width;
    private int height;
    private float cellSize;

    private GridSystem<PathNode> gridSystem;

    private void Awake() 
    {
        if(Instance != null)
        {
            Debug.Log("There is more than one instance of PathFinding" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width; 
        this.height = height;
        this.cellSize = cellSize;
        gridSystem = new GridSystem<PathNode>(width, height, cellSize, (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        for (int x = 0; x < width; x++)
        {   
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycastOffsetDistance = 5f;
                if(Physics.Raycast(worldPosition + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2, obstacleLayerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }


    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode lastNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost(); 

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCost(openList);

            if (currentNode == lastNode)
            {
                pathLength = lastNode.GetFCost();
                return CalculatePath(lastNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode adjacentNode in GetAdjacent(currentNode))
            {
                if(closedList.Contains(adjacentNode))
                {
                    continue;
                }

                if (!adjacentNode.IsWalkable())
                {
                    closedList.Add(adjacentNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), adjacentNode.GetGridPosition());
                if (tentativeGCost < adjacentNode.GetGCost())
                {
                    adjacentNode.SetCameFromPathNode(currentNode);
                    adjacentNode.SetGCost(tentativeGCost);
                    adjacentNode.SetHCost(CalculateDistance(adjacentNode.GetGridPosition(), endGridPosition));
                    adjacentNode.CalculateFCost();  

                    if(!openList.Contains(adjacentNode))
                    {
                        openList.Add(adjacentNode);
                    }
                }
            }
        }

        // if there is no found path
        pathLength = 0;
        return null;
        
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;

        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);

        return moveDiagonalCost * Mathf.Min(xDistance, zDistance) + moveStraightCost * remaining;

    }

    private PathNode GetLowestFCost(List<PathNode> pathNodeList)
    {
        PathNode lowestFCost = pathNodeList[0];
        for(int i = 0; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].GetFCost() < lowestFCost.GetFCost())
            {
                lowestFCost = pathNodeList[i];
            }
        }
        return lowestFCost;
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }

    private List <PathNode> GetAdjacent(PathNode currentNode)
    {
        List<PathNode> adjacentList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0)
        {
            // Left
            adjacentList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));

            // Left Down
            if(gridPosition.z - 1 >= 0)
            {
                adjacentList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }
            

            // Left Up
            if(gridPosition.z + 1 <  gridSystem.GetHeight())
            {
                adjacentList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
            }
            
            
        }
        if (gridPosition.x + 1 < gridSystem.GetWidth())
        {
            
            // Right
            adjacentList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));

            if(gridPosition.z - 1 >= 0)
            {
                // Right Down
                adjacentList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
            }
            
            if(gridPosition.z + 1 < gridSystem.GetHeight())
            {
                // Right Up
                adjacentList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));  
            }
            

        }      

         if (gridPosition.z - 1 >= 0)
        {
            // Down
            adjacentList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
        }
        if (gridPosition.z + 1 < gridSystem.GetHeight())
        {
            // Up
            adjacentList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
        }

        return adjacentList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach(PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }
        return gridPositionList;
    }


    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }


    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }

}
