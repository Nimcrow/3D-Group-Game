using UnityEngine;
using System.Collections;

public class SpotlightMover : MonoBehaviour
{
    public GameObject stage;  // reference to the stage object
    public float flashDuration = 0.5f;  // Duration of the spotlight flash

    private Renderer stageRenderer;  // renderer of the stage (to get bounds)
    private Vector3 leftPosition;
    private Vector3 middlePosition;
    private Vector3 rightPosition;

    void Start()
    {
        // Get the Renderer of the stage object to access its bounds
        if (stage != null)
        {
            stageRenderer = stage.GetComponent<Renderer>();
            if (stageRenderer == null)
            {
                Debug.LogError("Renderer component not found on the stage object!");
            }
        }
        else
        {
            Debug.LogError("Stage GameObject is not assigned!");
        }

        // Set fixed positions based on the stage's bounds
        SetFixedPositions();
    }

    void Update()
    {
        // Flash spotlight to a random position (middle, left, or right) when the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))  // Use space to trigger flash (for testing)
        {
            FlashSpotlight();
        }
    }

    void SetFixedPositions()
    {
        if (stageRenderer != null)
        {
            Bounds stageBounds = stageRenderer.bounds;

            middlePosition = new Vector3(
                stageBounds.center.x, // Middle of the stage
                transform.position.y, // Keep the current Y position
                stageBounds.center.z  // Keep Z at the center of the stage
            );

            leftPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                stageBounds.center.z - 6
            );

            rightPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                stageBounds.center.z + 6
            );
        }
    }

    void FlashSpotlight()
    {
        if (stageRenderer != null && gameObject != null)
        {
            // Randomly choose one of the fixed positions (left, middle, or right)
            Vector3 randomPosition = GetRandomPosition();

            // Move the spotlight to the chosen position
            transform.position = randomPosition;

            // Start the flash effect
            StartCoroutine(FlashEffect());
        }
    }

    Vector3 GetRandomPosition()
    {
        // Randomly pick one of the three fixed positions
        int randomIndex = Random.Range(0, 3);
        switch (randomIndex)
        {
            case 0:
                return leftPosition;
            case 1:
                return middlePosition;
            case 2:
                return rightPosition;
            default:
                return middlePosition;  // Default fallback to middle if something goes wrong
        }
    }

    IEnumerator FlashEffect()
    {
        Light light = GetComponent<Light>();
        if (light != null)
        {
            // Save the original intensity of the spotlight
            float originalIntensity = light.intensity;

            // Flash the spotlight (turn off the light for a short moment)
            light.intensity = 0;
            yield return new WaitForSeconds(0.1f);  // Flash duration

            light.intensity = originalIntensity;
            yield return new WaitForSeconds(flashDuration);
        }
        else
        {
            Debug.LogError("No Light component found on the spotlight.");
        }
    }
}
