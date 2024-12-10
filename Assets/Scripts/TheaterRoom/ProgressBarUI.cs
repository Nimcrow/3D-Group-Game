using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    public Image barImage;
    public float fillDuration = 10f;

    public SpotlightMover spotlightMover;

    private bool hasDanceMoveStarted = false; // Flag to ensure the dance move only starts once

    void Update()
    {
        if (spotlightMover.hasStart && !hasDanceMoveStarted)
        {
            StartCoroutine(DecrementBar());
            hasDanceMoveStarted = true;
        }
    }

    // to decrement the fill amount over time
    private IEnumerator DecrementBar()
    {
        while (true)
        {
            float currentTime = 0f;

            while (currentTime < fillDuration)
            {
                barImage.fillAmount = 1 - (currentTime / fillDuration);
                currentTime += Time.deltaTime; // increase by time passed since last frame

                // wait for next frame
                yield return null;
            }

            barImage.fillAmount = 0f;
            yield return null; // wait for next frame
            barImage.fillAmount = 1f;
        }
    }
}