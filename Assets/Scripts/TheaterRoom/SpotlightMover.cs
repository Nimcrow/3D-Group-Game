using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class SpotlightMover : MonoBehaviour
{
    public GameObject stage;
    public float flashDuration = 0.5f;

    private Renderer stageRenderer;  // (to get bounds)
    private Vector3 leftPosition;
    private Vector3 middlePosition;
    private Vector3 rightPosition;

    public bool leftPositionCam = false;
    public bool middlePositionCam = true;
    public bool rightPositionCam = false;

    private int lastCameraIndex = -1;
    public int spotlightSwitches = 0;

    public RandomAnimationController leadAnimationController;
    public GameObject leadModel;
    public ProgressBarUI progressBar;
    public MapDanceMove danceOptions;

    public GameObject playerModel;

    public bool hasStart = false;

    public SpotlightTrigger spotlightTrigger;

    public FinishPoint finishPoint1;
    public FinishPoint finishPoint2;

    void Start()
    {
        stageRenderer = stage.GetComponent<Renderer>(); // Get the Renderer of the stage object to access its bounds
    }

    void Update()
    {
        if (!hasStart) return;

        /* 
        1. when timer is over
        2. when player does wrong dance move
        3. when player is does right dance move
        */
        if (progressBar.barImage.fillAmount == 0) // start next spotlight
        {
            RotateLeadModel(leadModel); // rotate leadModel
            AdjustPlayerModelTransform(playerModel); // rotate playerModel
            leadAnimationController.RandomDanceMove();
            danceOptions.ResetMaterial(); // change all options to green
            FlashSpotlight();

            spotlightSwitches++;
        }
    }

    void RotateLeadModel(GameObject model)
    {
        model.transform.localEulerAngles = new Vector3(0, 90, -90); // x y z rotation to face audience
    }

    void AdjustPlayerModelTransform(GameObject model)
    {
        model.transform.localEulerAngles = new Vector3(0, 0, 0); // x y z rotation to face audience
    }


    public void StartMiniGame()
    {
        hasStart = true;
        SetFixedPositions();
    }

    public void StopMiniGame()
    {
        transform.position = new Vector3(1000f, transform.position.y, 1000f);
        StopCoroutine("FlashEffect");

        // go back to player camera
        spotlightTrigger.SwitchToMainCamera();

        // turn off booing audio
        spotlightTrigger.booSFX.Pause();

        finishPoint1.ActivateFinishPoint(); // spawn exit
        finishPoint2.ActivateFinishPoint(); // spawn exit
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
            stageBounds.center.z - 5
        );

        rightPosition = new Vector3(
            transform.position.x,
            transform.position.y,
            stageBounds.center.z + 5
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
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, 3); // random spotlight mover
        } while (randomIndex == lastCameraIndex); // spotlight never repeats position

        lastCameraIndex = randomIndex; // Update the lastRandomIndex to the current one for the next call

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
        float originalIntensity = light.intensity;

        light.intensity = 0;
        yield return new WaitForSeconds(0.1f);

        light.intensity = originalIntensity;
        yield return new WaitForSeconds(flashDuration);
    }
}