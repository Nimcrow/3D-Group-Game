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

    public bool leftPositionCam = false;
    public bool middlePositionCam = true; // spotlight starts in the middle
    public bool rightPositionCam = false;

    void Start()
    {
        stageRenderer = stage.GetComponent<Renderer>(); // Get the Renderer of the stage object to access its bounds

        SetFixedPositions();
    }

    void Update()
    {
        // Flash spotlight to a random position (middle, left, or right) when the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            FlashSpotlight();
        }
    }

    void SetFixedPositions()
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

    void FlashSpotlight()
    {
        Vector3 randomPosition = GetRandomPosition(); // Randomly pick one of the three fixed positions
        transform.position = randomPosition;

        StartCoroutine(FlashEffect());
    }

    Vector3 GetRandomPosition()
    {
        int randomIndex = Random.Range(0, 3);
        switch (randomIndex)
        {
            case 0:
                leftPositionCam = true;
                middlePositionCam = false;
                rightPositionCam = false;
                return leftPosition;
            case 1:
                leftPositionCam = false;
                middlePositionCam = true;
                rightPositionCam = false;
                return middlePosition;
            case 2:
                leftPositionCam = false;
                middlePositionCam = false;
                rightPositionCam = true;
                return rightPosition;
            default:
                leftPositionCam = false;
                middlePositionCam = true;
                rightPositionCam = false;
                return middlePosition;
        }
    }

    IEnumerator FlashEffect()
    {
        Light light = GetComponent<Light>();
        // Save the original intensity of the spotlight
        float originalIntensity = light.intensity;

        // Flash the spotlight (turn off the light for a short moment)
        light.intensity = 0;
        yield return new WaitForSeconds(0.1f);  // Flash duration

        light.intensity = originalIntensity;
        yield return new WaitForSeconds(flashDuration);
    }
}
