using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{

    private Action onInteractionFinish;

    private bool isActive;
    
    private float timer; 


    [SerializeField] private Material greenGlow;
    [SerializeField] private Material redGlow;

    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private bool isGreen;

    private GridPosition gridPosition;

    
    private void Start() 
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        SetColorGreen();    
    }
    private void SetColorGreen()
    {
        isGreen = true;
        meshRenderer.material = greenGlow;
    }

    private void SetColorRed()
    {
        isGreen = false;    
        meshRenderer.material = redGlow;
    }


    private void Update() 
    {
        if(!isActive)
        {
            return;
        }
        timer -= Time.deltaTime;    

        if (timer <= 0f)
        {
            isActive = false;   
            onInteractionFinish();
        }
    }


    public void Interact(Action onInteractionFinish)
    {
        this.onInteractionFinish = onInteractionFinish;
        isActive = true;
        timer = 0.5f;
        
        if(isGreen)
        {
            SetColorRed();
        }
        else
        {
            SetColorGreen();
        }
    }
}
