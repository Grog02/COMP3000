using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public Button pauseButton;
    
    
    [SerializeField]
    private bool isPaused = false;

    [SerializeField]
    public GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Call the custom function
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused; // Toggle the state of isPaused
        Time.timeScale = isPaused ? 0 : 1; // Pause or unpause the game based on isPaused state
        panel.SetActive(isPaused); // Show or hide the panel based on isPaused state
        Debug.Log("Pause Toggled: " + isPaused);
    }

    public void Home()
    {
        Time.timeScale = 1;
    }
}
