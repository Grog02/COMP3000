using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PathfindingGridDebugObject : GridDebugObject
{

    [SerializeField] private TextMeshPro gCostTest;
    [SerializeField] private TextMeshPro hCostTest;
    [SerializeField] private TextMeshPro fCostTest;

    [SerializeField] private SpriteRenderer isWalkableSprite;

    private PathNode pathNode;
    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        pathNode = (PathNode)gridObject;
        
    }

    private protected override void Update()
    {
        base.Update();
        gCostTest.text = pathNode.GetGCost().ToString();
        hCostTest.text = pathNode.GetHCost().ToString();
        fCostTest.text = pathNode.GetFCost().ToString();
        isWalkableSprite.color = pathNode.IsWalkable() ? Color.green : Color.red;
    }
}
