using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubToLevel : MonoBehaviour
{
    public string targetScene; // The name of the scene to transition to
    public float interactionDistance = 3.0f; // The distance within which the player can interact with the door
    private bool isPlayerNearby = false; // If the player is within interaction range
    private FirstPersonController playerController;

    void Start()
    {
        // Get the player's FirstPersonController component
        playerController = FindObjectOfType<FirstPersonController>();
    }

    void Update()
    {
        // Calculate the distance between the player and the door
        float distanceToDoor = Vector3.Distance(transform.position, playerController.transform.position);

        // Check if the player is within interaction range and presses the interaction key
        if (distanceToDoor <= interactionDistance && Input.GetKeyDown(playerController.interactKey))
        {
            // Call the function to change the scene
            ChangeScene();
        }
    }

    // Function to change the scene
    private void ChangeScene()
    {
        // Ensure the target scene is set
        if (!string.IsNullOrEmpty(targetScene))
        {
            // Trigger the scene transition
            SceneManager.LoadScene(targetScene);
        }
    }
}
