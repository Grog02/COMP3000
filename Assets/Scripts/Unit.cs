using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine; 

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 2;
    
    public static event EventHandler OnAnyActionPointsChanged;

    public static event EventHandler OnAnyUnitSpawned;

    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;
    private SpinAction spinAction;
    private GridPosition gridPosition;
    private MoveAction moveAction;
    private ShootAction shootAction;
    private BaseAction[] baseActionArray;
    private int actionPoints = ACTION_POINTS_MAX;
    private HealthSystem healthSystem;


    private void Awake() 
    {
        healthSystem = GetComponent<HealthSystem>();
        moveAction = GetComponent<MoveAction>();    
        spinAction = GetComponent<SpinAction>(); 
        shootAction = GetComponent<ShootAction>();
        baseActionArray = GetComponents<BaseAction>();
    }

    
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitatGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);

        
    }
    private void Update()
    {

        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition)
        {
            // Unit Change grid position
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;     

            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
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

    public ShootAction GetShootAction()
    {
        return shootAction;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
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

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            Debug.Log("action points " + actionPoints);
            actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Debug.Log("action points " + actionPoints);
        }
        
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this); 
        Destroy(gameObject);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalised();
    }
}