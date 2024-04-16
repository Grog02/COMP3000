#define USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InputManager : MonoBehaviour
{   
    public static InputManager Instance {get; private set;}

    private PlayerInput playerInput;    

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.Log("There is more than one instance of InputManager" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInput = new PlayerInput();
        playerInput.Player.Enable();
    }


    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else 
        return Input.mousePosition;
#endif
    }

    public bool isLeftMouseDownThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInput.Player.MouseClick.WasPressedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif
    }
    
    public Vector2 GetCameraMove()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInput.Player.CameraMovement.ReadValue<Vector2>();

#else

        Vector2 inputMoveDirection = new Vector2(0,0);

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDirection.y = +1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDirection.x = -1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDirection.y = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDirection.x = +1f;
        }

        return inputMoveDirection;
#endif
    }

    public float GetCameraRotation()
    {
#if USE_NEW_INPUT_SYSTEM  
        return playerInput.Player.CameraRotation.ReadValue<float>();      

#else
        float rotation = 0f;
        
        if(Input.GetKey(KeyCode.Q))
        {
            rotation =+ 1f;
        }
        if(Input.GetKey(KeyCode.E))
        {
            rotation =- 1f;
        }

        return rotation;
#endif
    }

    public float GetCameraZoom()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInput.Player.CameraZoom.ReadValue<float>();
#else
        float zoomAmount = 0f;
        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount =- 1f;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount =+ 1f;
        }

        return zoomAmount;
#endif
    }

    public Vector2 GetMinimapMove()
{
#if USE_NEW_INPUT_SYSTEM
    return playerInput.Player.MiniMapMovement.ReadValue<Vector2>();
#else
    // Implement the old input system handling if needed
    Vector2 inputMoveDirection = new Vector2(0,0);

    if (Input.GetKey(KeyCode.W))
    {
        inputMoveDirection.y = +1f;
    }
    if (Input.GetKey(KeyCode.A))
    {
        inputMoveDirection.x = -1f;
    }
    if (Input.GetKey(KeyCode.S))
    {
        inputMoveDirection.y = -1f;
    }
    if (Input.GetKey(KeyCode.D))
    {
        inputMoveDirection.x = +1f;
    }


    return inputMoveDirection;
#endif
}
}
