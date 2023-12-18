using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }


    private enum State
    {
        Aiming,
        Shooting,
        Cooldown,
    }
    private State state;

    private int maxShootDistance = 7;

    private float stateTimer;

    private Unit targetUnit;

    private bool canShoot;
    private void Update() 
    {
        if(!isActive)
        {
            return;
        }
        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.Aiming:
            Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if(canShoot)
                {
                    Shoot();
                    canShoot = false;
                }
                break;
            case State.Cooldown:
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
            case State.Aiming:
                if(stateTimer <= 0f)
                {
                    state = State.Shooting;
                    float shootingStateTime = 0.1f;
                    stateTimer = shootingStateTime;
                } 
            break;
            case State.Shooting:
                if(stateTimer <= 0f)
                {
                    state = State.Cooldown;
                    float coolDownStateTime = 1f;
                    stateTimer = coolDownStateTime;
                } 
            break;
            case State.Cooldown:
                ActionComplete();
            break;
        }
    }


    private void Shoot()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });
        targetUnit.Damage(40);
    }


    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        


        for (int x =- maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z =- maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    //inside the grid or not
                    continue;   
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if(testDistance > maxShootDistance)
                {
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
                //Debug.Log(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        
        
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShoot = true;
        ActionStart(onActionComplete);
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        
        return new EnemyAIAction{
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
