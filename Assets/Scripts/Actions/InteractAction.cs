using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    // Range for InteractAction
    private int maxInteractDistance = 1;


    private void Update()
    {
        // Is action active
        if(!isActive)
        {
            return;
        }
        
    }
    // Action Name
    public override string GetActionName()
    {
        return "Interact";
    }

    // Enemy AI Action
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction{
            gridPosition = gridPosition,
            actionValue = 0
        };
    }
    
    // Valid Grid Positions for Interact Action
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        // Store valid grid positions
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        // Units current position
        GridPosition unitGridPosition = unit.GetGridPosition();

        // Loop through grid positions in range 
        for (int x =- maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z =- maxInteractDistance; z <= maxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    //inside the grid or not
                    continue;   
                }
                
                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
                if(interactable== null)
                {
                    // No interactable found
                    continue; 
                }
                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(onInteractionFinish);
        ActionStart(onActionComplete);
    }
    private void onInteractionFinish()
    {
        ActionComplete();
    }
}
