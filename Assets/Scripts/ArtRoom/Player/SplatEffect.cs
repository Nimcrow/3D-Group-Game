using UnityEngine;
using UnityEngine.UI;

public class SplatEffect : MonoBehaviour
{
    public Image splatImage; // Reference to the UI Image for the splat
    public float splatDuration = 2f; // Duration the splat stays on screen

    [SerializeField] private AudioClip splatSound; // Reference to the splat sound effect
    private AudioSource audioSource;

    private void Start()
    {
        if (splatImage != null)
        {
            splatImage.enabled = false; // Ensure the splat is initially hidden
        }

        // Ensure an AudioSource is attached or add one
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void ShowSplat()
    {
        if (splatImage != null)
        {
            splatImage.enabled = true; // Show the splat

            // Play the splat sound
            PlaySplatSound();

            // Hide it after the duration
            Invoke(nameof(HideSplat), splatDuration);
        }
    }

    private void HideSplat()
    {
        if (splatImage != null)
        {
            splatImage.enabled = false; // Hide the splat
        }
    }

    private void PlaySplatSound()
    {
        if (splatSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(splatSound); // Play the splat sound
        }
    }
}
