using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    private void Start() 
    {
        Crate.OnAnyCrateDestroy += OnAnyCrateDestroy_onAnyCrateDestroy;    
    }

    private void OnAnyCrateDestroy_onAnyCrateDestroy(object sender, EventArgs e)
    {
        Crate crate = sender as Crate;
        PathFinding.Instance.SetIsWalkableGridPosition(crate.GetGridPosition(), true);
    }
}
