using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsAppear : MonoBehaviour
{
        public TextMeshProUGUI textMeshPro;
    public Button button; // Assign this in the inspector
    private CanvasGroup buttonCanvasGroup; // To control the button's fade in and out
    private float fadeInDuration = 1.0f; 
    private float fadeOutDuration = 1.0f; 
    private float visibleDuration = 5.0f;

    private void Awake()
    {
        // Ensure the button has a CanvasGroup component
        buttonCanvasGroup = button.GetComponent<CanvasGroup>();
        if (buttonCanvasGroup == null) // If there's no CanvasGroup, add one
        {
            buttonCanvasGroup = button.gameObject.AddComponent<CanvasGroup>();
        }
        
        // Initialize the TextMeshProUGUI element as inactive
        textMeshPro.gameObject.SetActive(false);
    }

    public void Push()
    {
        // Activate the text and start the sequence
        textMeshPro.gameObject.SetActive(true);
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        // No need to fade out the button in this new setup

        // Fade in the text
        yield return StartCoroutine(FadeTextToFullAlphaThenFadeOut(fadeInDuration, fadeOutDuration, textMeshPro));

        // The button stays interactive throughout, so no changes here are necessary
    }

    public IEnumerator FadeTextToFullAlphaThenFadeOut(float fadeInTime, float fadeOutTime, TextMeshProUGUI textElement)
    {
        // Set the initial conditions for the text element
        textElement.text = "ESC - Pause\nTab - Check Map\nWASD - Move Camera\nQ/E - Rotate Camera\nScroll - Zoom in/out\nDestroy Crates to get extra health!";
        textElement.color = new Color(textElement.color.r, textElement.color.g, textElement.color.b, 0);

        // Fade in the text
        yield return StartCoroutine(FadeAlpha(textElement, 0, 1, fadeInTime));

        // Keep the text visible
        yield return new WaitForSeconds(visibleDuration);

        // Fade out the text
        yield return StartCoroutine(FadeAlpha(textElement, 1, 0, fadeOutTime));

        // After fading out, set the text to inactive
        textElement.gameObject.SetActive(false);
    }

    private IEnumerator FadeAlpha(TextMeshProUGUI textElement, float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            textElement.color = new Color(textElement.color.r, textElement.color.g, textElement.color.b, Mathf.Lerp(startAlpha, endAlpha, time / duration));
            yield return null;
        }
        textElement.color = new Color(textElement.color.r, textElement.color.g, textElement.color.b, endAlpha);
    }

}
