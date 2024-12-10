using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private AudioSource audioSource;  // Reference to the AudioSource component (for rolling sound)
    private AudioSource splatterAudioSource;  // Reference to the AudioSource component (for splatter sound)
    public AudioClip splatterSound;   // The splatter sound clip

    private void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
        
        // Assuming there's a second AudioSource for the splatter sound attached to the same GameObject
        splatterAudioSource = gameObject.AddComponent<AudioSource>();

        // Check if the AudioSource component exists
        if (audioSource != null)
        {
            // Set the AudioSource to loop the sound
            audioSource.loop = true;

            // Play the rolling sound immediately when the rock is created
            audioSource.Play();
        }

        // Set the splatter sound to the second AudioSource
        if (splatterAudioSource != null && splatterSound != null)
        {
            splatterAudioSource.clip = splatterSound;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Rock and Player have collided");

            // Access GameManager and drop the grade
            GameManager.Instance.DropLetterGrade();
            Debug.Log($"New letter grade: {GameManager.Instance.letterGrade}");

            // Play the splatter sound when the collision happens
            if (splatterAudioSource != null && splatterAudioSource.clip != null)
            {
                splatterAudioSource.Play();  // Play the splatter sound once
            }
        }
    }
}
