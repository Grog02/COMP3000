using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    // All possible states for enemies to be in
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    }

    private State state;
    private float timer;

    private void Awake() 
    {
        // Always start waiting for turn
        state = State.WaitingForEnemyTurn;
    }

    private void Start() 
    {
        // Subscribe to event when turn changes to enemy turn
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged; 
    }

    private void Update() 
    {
        if (TurnSystem.Instance.IsPlayerTurn())    
        {
            return;
        }
        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;

            case State.TakingTurn:
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    
                    if(TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        // If no more enemies can take actions go to next turn
                        TurnSystem.Instance.NextTurn();
                    }
                    
                    
                }
                break;
            case State.Busy:
                // Enemy is performing action
                break;
        }
        
    }

    private void SetStateTakingTurn()
    {
        // Time between moves so they do not happen instantly after one move finishes 
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged (object sender, EventArgs e)
    {
        if(!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            // Timer before taking move so enemy's turn doesn't start instantly
            timer = 2f;
        }
    }

    // Try to make a move
    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        foreach(Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if(TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }
        return false;
    }

    // Try to make a move for a specific unit
    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;
        foreach(BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            // If enemy has enough action points or not
            if(!enemyUnit.CanSpendActionPoints(baseAction))
            {
                continue;
            }
            // Get best enemy ai move based on value
            if(bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if(testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
            
            
        }

        // If best move is found and enemy has enough action points, make the move
        if(bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToPerformAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }
}
