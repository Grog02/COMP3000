using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

public class ResultDecider : MonoBehaviour
{
    private bool lose = false;
    private bool win = false;
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private Image black;

    public float fadeDuration = 5;

    private void Awake() 
    {
        GameOverScreen.SetActive(false);
        WinScreen.SetActive(false);
    }

    private void Start() 
    {
        StartCoroutine(WaitForThreeSeconds());
    
    }

    IEnumerator WaitForThreeSeconds()
    {
        yield return new WaitForSeconds(3);
        
    }
    private void OnEnable()
    {
        // Subscribe to the events
        HealthSystem.OnLose += HandleLose;
        HealthSystem.OnWin += HandleWin;
    }

    private void OnDisable()
    {
        // Unsubscribe from the events
        HealthSystem.OnLose -= HandleLose;
        HealthSystem.OnWin -= HandleWin;
    }

    // Method to handle the Lose event
    private void HandleLose()
    {
        lose = true;
        Debug.Log("Game Over: You Lose!");
        StartCoroutine(DelayedResultDisplay(false));

        // Here you can add more logic for when the game is lost,
        // such as displaying a lose screen, playing sounds, etc.
    }

    // Method to handle the Win event
    private void HandleWin()
    {
        win = true;
        Debug.Log("Congratulations: You Win!");
        StartCoroutine(DelayedResultDisplay(true));
        // Here you can add more logic for when the game is won,
        // such as displaying a win screen, playing victory sounds, etc.
    }

    IEnumerator DelayedResultDisplay(bool win)
    {
    yield return new WaitForSeconds(3); // Wait for 3 seconds

    if(win)
    {
        
        yield return StartCoroutine(FadeScreen(Color.black, fadeDuration));
        WinScreen.SetActive(true);
        Time.timeScale = 0; 
    }
    else
    {
        yield return StartCoroutine(FadeScreen(Color.black, fadeDuration));
        GameOverScreen.SetActive(true);
        Time.timeScale = 0; 
    }
    }

    private IEnumerator FadeScreen(Color targetColor, float duration)
{
    black.gameObject.SetActive(true);
    black.color = Color.clear;
    float timer = 0f;
    Image imageComponent = black.GetComponent<Image>(); // Get the Image component

    Color startColor = imageComponent.color; // Get the initial color of the screen

    while (timer < duration)
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration); // Ensure t is between 0 and 1

        // Interpolate between startColor and targetColor based on t
        Color currentColor = Color.Lerp(startColor, targetColor, t);
        
        // Set the color of the screen
        imageComponent.color = currentColor;

        yield return null; // Wait for the next frame
    }
    
    // Ensure the final color is exact
    imageComponent.color = Color.black; 
    
}
}
