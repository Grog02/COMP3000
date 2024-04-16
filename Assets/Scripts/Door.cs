using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{

    [SerializeField] private bool doorIsOpen;

    private GridPosition gridPosition;

    private Animator animator;
    private Action onInteractionFinish;

    public static event EventHandler OnAnyDoorOpened;
    public event EventHandler OnDoorOpened;

    private bool isActive;
    
    private float timer; 

    private void Awake() 
    {
        animator = GetComponent<Animator>();

    }
    
    private void Start() 
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        if (doorIsOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
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
        if (doorIsOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }   

    

    private void Open()
    {
        doorIsOpen = true;
        PathFinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
        animator.SetBool("IsDoorOpen", doorIsOpen);

        OnDoorOpened?. Invoke (this,EventArgs.Empty);
    }
    private void Close()
    {
        doorIsOpen = false; 
        PathFinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
        animator.SetBool("IsDoorOpen", doorIsOpen);
    }
}
