using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    // Event triggered when any action starts
    public event EventHandler OnStartMoving;

    // Event triggered when any action is completed
    public event EventHandler OnStopMoving;

    // Maximum distance that the unit can move
    [SerializeField] private int maxMoveDistance = 4;

    // List of positions that the unit can move to
    private List<Vector3> positionList;

    // Index of the current position
    private int currentPositionIndex;

    //
    private void Update() 
    {
        if(!isActive) 
        {
            return;
        }


        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        // Rotate towards the target position 
        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        // Minimum stopping distance
        float stoppingDistance = .1f;
        if(Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {        

            float moveSpeed = 4f;
            // Move towards the target position 
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

        }
        else
        {
            currentPositionIndex ++;
            if(currentPositionIndex >= positionList.Count)
            {
                // Trigger the event and stop moving
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }

        }

    }


    // Execute movement
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        // Find path to target position and calculate distance
        List<GridPosition> pathGridPositionList = PathFinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        // Convert grid position to world position
        foreach(GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        // Trigger the start moving event 
        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
        Debug.Log("moving");
    }

    // Gets a list of the valid grid positions
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        
        GridPosition unitGridPosition = unit.GetGridPosition();

        // Check all grid positions within the distance and check the validity 
        for (int x =- maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z =- maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                
                // Checks to see if grid position is valid
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    //inside the grid or not
                    continue;
                }
                if (unitGridPosition == testGridPosition)
                {
                    //same grid position as current player
                    continue;
                }

                if (LevelGrid.Instance.HasUnitOnGridPosition(testGridPosition))
                {
                    //grid position used by another unit
                    continue;
                }

                if(!PathFinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if(!PathFinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }

                int pathFindingMultiplier = 10;
                if(PathFinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathFindingMultiplier)
                {
                    // Path length is too long
                    continue;
                }
                

                validGridPositionList.Add(testGridPosition);
                //Debug.Log(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    // Returns name of action
    public override string GetActionName()
    {
        return "Move";
    }

    // Calculates the best enemy AI action at the given grid position
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction{
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
}
