using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{

    //Singleton
    public static UnitActionSystem Instance {get; private set;}
    
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStart;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;


    private BaseAction selectedAction;

    private bool isBusy;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.Log("There is more than one instance of UnitActionSystem" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() 
    {
        SetSelectedUnit(selectedUnit);    
    }
    private void Update()
    {
        if(isBusy)
        {
            return;
        }

        if(!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (TryHandleSelection())
        {
            return;
        }
        
        HandleSelectedAction();
        
    }
    
    private void HandleSelectedAction()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(Mouse.GetPosition());


            if(selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                if(selectedUnit.TrySpendActionPointsToPerformAction(selectedAction))
                {

                    SetBusy();
                    selectedAction.TakeAction(mouseGridPosition, ClearBusy);
                    OnActionStart?.Invoke(this, EventArgs.Empty);
                }

            }
        }
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }
    private bool TryHandleSelection()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if(unit == selectedUnit)
                    {
                        // unit already selected
                        return false;
                    }
                    if (unit.IsEnemy())
                    {
                        // clicked on enemy unit
                        return false;
                    }
                    SetSelectedUnit(unit);
                    return true;
                }
            }   
        }
        return false;
    }
    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetMoveAction());
        //Creates event
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty); 
    }
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
}
