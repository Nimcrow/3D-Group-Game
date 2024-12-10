using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    public Image barImage;
    public float fillDuration = 10f;

    public SpotlightMover spotlightMover;

    private void Start()
    {
        gameObject.SetActive(false); // disable entire game object until start
        if(spotlightMover.hasStart)
            StartCoroutine(DecrementBar());
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