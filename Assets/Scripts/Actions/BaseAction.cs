using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    // Event triggered when any action starts
    public static event EventHandler OnAnyActionStarted;

    // Event triggered when any action is completed
    public static event EventHandler OnAnyActionCompleted;

    // Bool to check if action is currently active
    protected bool isActive;

    // Reference to the Unit performing the action
    protected Unit unit;

    protected Action onActionComplete;

    protected virtual void Awake() 
    {
        unit = GetComponent<Unit>();    
    }

    public abstract string GetActionName();

    // Execute action using a specific grid position
    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    // Checks if the chosen grid position is valid in the grid
    public bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }
    
    // Returns a list of the valid grid positions for this action
    public abstract List<GridPosition> GetValidActionGridPositionList();

    // Cost of this action
    public virtual int GetActionPointsCost()
    {
        return 1;
    }

    // Executes the action
    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    // Triggers the action completed event
    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    // Gets the Unit making the action
    public Unit GetUnit()
    {
        return unit;
    }

    // Determines the best action for an Enemy based on the available grid positions
    public EnemyAIAction GetBestEnemyAIAction()
    {
        // List of potential actions
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();
        // Get currently valid grid positions where the action can be executed
        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();
        // Look through each grid position to evaluate which is best
        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }
        // Sort list by its action value and then choosing the best value
        if(enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
        }
        else
        {
            //no possible actions
            return null;
        }
        // return best action from the list 
        return enemyAIActionList[0];
    }
    
    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
}
