using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public GameObject miniMapCamera;

    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the tab key is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Toggle the isActive state
            isActive = !isActive;

            // Set the objectToToggle active state accordingly
            miniMapCamera.SetActive(isActive);
            Vector2 inputMoveDirection = InputManager.Instance.GetMinimapMove();
        }
    }
}
