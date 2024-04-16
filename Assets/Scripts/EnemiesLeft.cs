using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class EnemiesLeft : MonoBehaviour
{
    private HealthSystem healthSystem;
    private int enemiesLeft = 9;
    private int unitsLeft = 5;
    private bool lose = false;
    private bool win = false;

    public TextMeshProUGUI enemiesLeftText;
    [SerializeField] private GameObject GameOverScreenImage;
    [SerializeField] private GameObject WinScreenImage;
    [SerializeField] private Image black;

    public float fadeDuration = 5; // Duration of fading animation

    private void Start()
    {
        UpdateEnemiesLeftText();
        WinScreenImage.SetActive(false);
        GameOverScreenImage.SetActive(false);
        black.gameObject.SetActive(false);
        healthSystem = FindObjectOfType<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("HealthSystem component not found in the scene.");
            return;
        }
    }

    public void DecreaseEnemyCount()
    {
        HandleEnemyDead();
    }

    public void DecreaseUnitCount()
    {
        HandleUnitDead();
    }

private IEnumerator DelayBeforeFadeScreenToBlack(float delay)
{
    yield return new WaitForSeconds(delay); // Wait for the specified delay
    if(win)
    {
        StartCoroutine(FadeScreenToBlackAndShowWinScreen()); // Start the fade screen coroutine after the delay
    }
    if (lose)
    {
        StartCoroutine(FadeScreenToBlackAndShowDefeatScreen()); // Start the fade screen coroutine after the delay
    }
    
}

private void HandleEnemyDead()
{
    enemiesLeft--;
    UpdateEnemiesLeftText();
    Debug.Log("Enemies left: " + enemiesLeft);
    if (enemiesLeft == 0)
    {
        UpdateEnemiesLeftText();
        float delayBeforeFade = 1f; // Adjust this value as needed
        StartCoroutine(DelayBeforeFadeScreenToBlack(delayBeforeFade));
        win = true;
    }
}

private void HandleUnitDead()
{
    unitsLeft--;
    if (unitsLeft == 0)
    {
        float delayBeforeFade = 1f; // Adjust this value as needed
        StartCoroutine(DelayBeforeFadeScreenToBlack(delayBeforeFade));
        lose = true;
    }
}

private IEnumerator FadeScreenToBlackAndShowWinScreen()
{
    // Fade the screen to black
    yield return StartCoroutine(FadeScreen(Color.black, fadeDuration));

    // Wait for a short duration before showing the win screen
    yield return new WaitForSeconds(1f);

    // Show the win screen
    WinScreenImage.SetActive(true);

    // Fade the screen back in
    yield return StartCoroutine(FadeScreen(Color.clear, fadeDuration));
}

private IEnumerator FadeScreenToBlackAndShowDefeatScreen()
{
    // Fade the screen to black
    yield return StartCoroutine(FadeScreen(Color.black, fadeDuration));

    // Wait for a short duration before showing the win screen
    yield return new WaitForSeconds(1f);

    // Show the win screen
    GameOverScreenImage.SetActive(true);

    // Fade the screen back in
    yield return StartCoroutine(FadeScreen(Color.clear, fadeDuration));
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


private void SetScreenColor(Color color)
{
    // Ensure there is an image component attached to the WinScreenImage or GameOverScreenImage GameObjects
    Image imageComponent = WinScreenImage.GetComponent<Image>();
    if (imageComponent == null)
    {
        imageComponent = GameOverScreenImage.GetComponent<Image>();
    }

    // Update the color of the image component
    if (imageComponent != null)
    {
        imageComponent.color = color;
    }
    else
    {
        Debug.LogError("No Image component found on WinScreenImage or GameOverScreenImage GameObject.");
    }
}


    private void UpdateEnemiesLeftText()
    {
        if (enemiesLeftText != null)
        {
            enemiesLeftText.text = "ENEMIES LEFT: " + enemiesLeft;
        }
    }
}
