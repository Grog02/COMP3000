using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    // Events for sword action starting and finishing
    public event EventHandler OnSwordActionStart;
    public event EventHandler OnSwordActionFinish;
    // Event for when the sword hits the target
    public static event EventHandler OnSwordHit;

    // States for sword
    private enum State
    {
        SwordBeforeHit,
        SwordAfterHit,
    }
    // Range of sword action 
    private int maxSwordSwingDistance = 1;
    private State state;
    private float stateTimer;

    // Unit to be hit by sword
    private Unit targetUnit;

    private void Update()
    {
        // Is action active
        if(!isActive)
        {
            return;
        }
        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.SwordBeforeHit:
                // Turn to face target before attacking 
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.SwordAfterHit:
                break;
        }

        if(stateTimer <= 0f)
        {
            NextState();
        } 
    }

    // Go to next state 
    private void NextState()
    {
        switch (state)
        {
            case State.SwordBeforeHit:      
                // Move to next state after hitting target        
                state = State.SwordAfterHit;
                float afterSwordStateTime = 0.5f;
                stateTimer = afterSwordStateTime;
                // Damage target unit 100
                targetUnit.Damage(100);
                // Trigger sword hit event
                OnSwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwordAfterHit:
                // Invoke sword action finish and complete the action
                OnSwordActionFinish?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    // Action name
    public override string GetActionName()
    {
        return "Sword";
    }


    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200
        };
    }

    // Grid position validation within range
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        // Store grid positions
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        // Units current grid position
        GridPosition unitGridPosition = unit.GetGridPosition();

        // Loop through all grid positions within range
        for (int x =- maxSwordSwingDistance; x <= maxSwordSwingDistance; x++)
        {
            for (int z =- maxSwordSwingDistance; z <= maxSwordSwingDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    //inside the grid or not
                    continue;   
                }

                
                if (!LevelGrid.Instance.HasUnitOnGridPosition(testGridPosition))
                {
                    //grid position is empty
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if(targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    // both units on same team
                    continue;
                }


                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        state = State.SwordBeforeHit;
        float BeforeSwordStateTime = 0.7f;
        stateTimer = BeforeSwordStateTime;
        OnSwordActionStart?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);

    }

    public int GetMaxSwordSwingDistance()
    {
        return maxSwordSwingDistance;
    }
}
