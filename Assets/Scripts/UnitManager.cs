using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{

    public static UnitManager Instance {get; private set;}

    private List<Unit> unitList;
    
    private List<Unit> friendlyList;

    private List<Unit> enemyList;


    private void Awake() 
    {
        if(Instance != null)
        {
            Debug.Log("There is more than one instance of UnitManager" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        unitList = new List<Unit>();
        friendlyList = new List<Unit>();
        enemyList = new List<Unit>();
    }
    private void Start() 
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {

        Unit unit = sender as Unit;
        unitList.Add(unit);
        if(unit.IsEnemy())
        {
            enemyList.Add(unit);
        }
        else
        {
            friendlyList.Add(unit);
        }

    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {

        Unit unit = sender as Unit;

        unitList.Remove(unit);
        if(unit.IsEnemy())
        {
            enemyList.Remove(unit);
        }
        else
        {
            friendlyList.Remove(unit);
        }

    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }
    
    public List<Unit> GetFriendlyUnitList()
    {
        return friendlyList;
    }

    public List<Unit> GetEnemyUnitList()
    {
        return enemyList;
    }


}
