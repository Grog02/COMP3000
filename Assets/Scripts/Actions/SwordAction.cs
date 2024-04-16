using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public event EventHandler OnSwordActionStart;
    public event EventHandler OnSwordActionFinish;
    public static event EventHandler OnSwordHit;
    private enum State
    {
        SwordBeforeHit,
        SwordAfterHit,
    }
    private int maxSwordSwingDistance = 1;
    private State state;
    private float stateTimer;

    private Unit targetUnit;

    private void Update()
    {
        if(!isActive)
        {
            return;
        }
         stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.SwordBeforeHit:
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

    private void NextState()
    {
        switch (state)
        {
            case State.SwordBeforeHit:              
                state = State.SwordAfterHit;
                float afterSwordStateTime = 0.5f;
                stateTimer = afterSwordStateTime;
                targetUnit.Damage(100);
                OnSwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwordAfterHit:
                OnSwordActionFinish?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }


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

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        
        GridPosition unitGridPosition = unit.GetGridPosition();

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
