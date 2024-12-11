using UnityEngine;

public class MapDanceMove : MonoBehaviour
{
    public GameObject[] mappedObjects = new GameObject[6];  // dance choices

    public GameObject playerModel;  // player model
    private RuntimeAnimatorController defaultPlayerAnimatorController; // store the default AnimatorController for the player

    public Material incorrectMaterial; // red for chosen
    public Material correctMaterial; // for resetting after spotlight switch

    public GameObject leadModel;  // lead model to follow
    private RuntimeAnimatorController expectedDanceController;

    public SpotlightMover spotlightMover;

    private string[] grades = { "A", "B", "C", "D", "F" };
    private int gradeIndex = 0; // Index for current grade

    public GradeDisplayTheater gradeController;

    void Start()
    {
        // Store the default player animation at the start
        Animator playerAnimator = playerModel.GetComponent<Animator>();
        if (playerAnimator != null)
            defaultPlayerAnimatorController = playerAnimator.runtimeAnimatorController;
    }

    void Update()
    {
        if (spotlightMover.hasStart)
        {
            // Loop through keys 1-6 to detect key presses
            for (int i = 0; i < 6; i++)
            {
                if (Input.GetKeyDown((KeyCode)(KeyCode.Alpha1 + i))) // maps 1 -> 6 to keys 1 to 6
                {
                    // change the material for the selected banana man (based on the index)
                    ChangeMaterial(i);

                    // trigger the animation for the selected banana man (based on the index)
                    TriggerDanceMove(i);
                }
            }
        }

    }

    // change the material of the selected banana man to selected
    public void ChangeMaterial(int objectIndex)
    {
        if (objectIndex < 0 || objectIndex >= mappedObjects.Length) return; // Ensure the index is within range

        GameObject selectedObject = mappedObjects[objectIndex];

        Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>();

        // loop through all the renderers in the children and change their material
        foreach (Renderer renderer in renderers)
        {
            renderer.material = incorrectMaterial;
        }
    }

    // reset the material of all objects back to the correct material
    public void ResetMaterial()
    {
        for (int objectIndex = 0; objectIndex < mappedObjects.Length; objectIndex++)
        {
            GameObject selectedObject = mappedObjects[objectIndex];
            Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                renderer.material = correctMaterial;
            }
        }
    }

    // trigger the dance move animation for the player based on the selected banana man
    public void TriggerDanceMove(int objectIndex)
    {
        if (objectIndex < 0 || objectIndex >= mappedObjects.Length) return; // Ensure the index is within range

        // get the Animator component from the selected dance option
        Animator selectedDanceAnimator = mappedObjects[objectIndex].GetComponent<Animator>();
        Animator expectedAnimator = leadModel.GetComponent<Animator>(); // for grading
        Animator playerAnimator = playerModel.GetComponent<Animator>();

        // display chosen animation/dance
        if (selectedDanceAnimator != null)
        {
            playerAnimator.applyRootMotion = false; // to stop the player from moving

            if (playerAnimator != null)
                playerAnimator.runtimeAnimatorController = selectedDanceAnimator.runtimeAnimatorController; // change animation

            if (playerAnimator.runtimeAnimatorController.name == "anim.headspin")
                playerAnimator.applyRootMotion = true;
            if (playerAnimator.runtimeAnimatorController.name == "anim.maraschino")
                playerAnimator.applyRootMotion = true;
        }

        // check if chosen dance matches expected dance
        if (expectedAnimator.runtimeAnimatorController.name != playerAnimator.runtimeAnimatorController.name)
        {
            UpdateGrade(); // bad
            gradeController.RefreshGrade(); // reflect grade in UI
        }
    }

    private void UpdateGrade()
    {
        if (gradeIndex < grades.Length - 1)
        {
            gradeIndex++;
            Debug.Log($"Grade decreased! New grade: {grades[gradeIndex]}");
        }
        else
        {
            Debug.Log("Player has already reached the lowest grade (F).");
        }
    }

    public string GetCurrentGrade()
    {
        return grades[gradeIndex];
    }

    // reset the player's animation to its default controller (after exiting the spotlight)
    public void ResetPlayerAnimation()
    {
        Animator playerAnimator = playerModel.GetComponent<Animator>();
        playerAnimator.applyRootMotion = true;

        if (playerAnimator != null && defaultPlayerAnimatorController != null)
        {
            // reset to the default player animation controller
            playerAnimator.runtimeAnimatorController = defaultPlayerAnimatorController;
        }
    }
}