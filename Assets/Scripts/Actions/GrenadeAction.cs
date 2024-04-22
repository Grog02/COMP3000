using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;

public class GrenadeAction : BaseAction
{

    // Prefab for the grenade 
    [SerializeField] private Transform grenadePrefab;
    // Layer mask for obstacles to block grenade
    [SerializeField] private LayerMask obstacleLayerMask;
    // Grenade range
    private int maxThrowDistance = 7;
    private void Update() 
    {
        if(!isActive)    
        {
            return; 
        }
    }


    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction{gridPosition = gridPosition, actionValue = 0};
    }

    // List to get valid grenade range grid positions 
    public override List<GridPosition> GetValidActionGridPositionList()
    {
    List<GridPosition> validGridPositionList = new List<GridPosition>();
    // Get Units height to throw grenade
    GridPosition unitGridPosition = unit.GetGridPosition();
    Vector3 unitWorldPosition = unit.GetWorldPosition() + Vector3.up * 1.7f; 

    // Loop through grenade range 
    for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
    {
        for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
        {
            GridPosition offsetGridPosition = new GridPosition(x, z);
            GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
            {
                // Is valid grid position
                continue;   
            }

            int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
            if (testDistance > maxThrowDistance)
            {
                // Outside of throw range
                continue;
            }

            Vector3 targetWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
            Vector3 direction = (targetWorldPosition - unitWorldPosition).normalized;
            float distance = Vector3.Distance(unitWorldPosition, targetWorldPosition);

            // Checking for obstacles in the way
            if (!Physics.Raycast(unitWorldPosition, direction, distance, obstacleLayerMask))
            {
                // No obstacle detected, add as valid position
                validGridPositionList.Add(testGridPosition);
            }
        }
    }
    return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform grenadeTransform = Instantiate(grenadePrefab, unit.GetWorldPosition(), quaternion.identity);
        Grenade grenade = grenadeTransform.GetComponent<Grenade>(); 
        grenade.Setup(gridPosition, OnGrenadeImpact);
        Debug.Log("Grenade Action");
        ActionStart(onActionComplete);
    }
    
    private void OnGrenadeImpact()
    {
        ActionComplete();
    }
}
