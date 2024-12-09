using UnityEngine;
using UnityEngine.UI;

public class SplatEffect : MonoBehaviour
{
    public Image splatImage; // Reference to the UI Image for the splat
    public float splatDuration = 2f; // Duration the splat stays on screen

    private void Start()
    {
        if (splatImage != null)
        {
            splatImage.enabled = false; // Ensure the splat is initially hidden
        }
    }

    public void ShowSplat()
    {
        if (splatImage != null)
        {
            splatImage.enabled = true; // Show the splat
            Invoke(nameof(HideSplat), splatDuration); // Hide it after the duration
        }
    }

    private void HideSplat()
    {
        if (splatImage != null)
        {
            splatImage.enabled = false; // Hide the splat
        }
    }
}
