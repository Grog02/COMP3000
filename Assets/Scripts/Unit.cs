using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class Unit : MonoBehaviour
{
    
    private SpinAction spinAction;
    private GridPosition gridPosition;
    private MoveAction moveAction;
    private BaseAction[] baseActionArray;
    private int actionPoints = 2;

    private void Awake() 
    {
        moveAction = GetComponent<MoveAction>();    
        spinAction = GetComponent<SpinAction>(); 
        baseActionArray = GetComponents<BaseAction>();
    }

    
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitatGridPosition(gridPosition, this);
    }
    private void Update()
    {

        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition)
        {
            // Unit Change grid position
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;     
        }
    }

    public MoveAction GetMoveAction()
    {
        return moveAction;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
    public SpinAction GetSpinAction()
    {
        return spinAction;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray; 
    }

    public bool TrySpendActionPointsToPerformAction(BaseAction baseAction)
    {
        if(CanSpendActionPoints(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false; 
        }
    }

    public bool CanSpendActionPoints(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

}