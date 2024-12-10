using UnityEngine;

public class MapDanceMove : MonoBehaviour
{
    public GameObject[] mappedObjects = new GameObject[6];  // Array of banana men
    public GameObject playerModel;  // Reference to the player model

    public Material incorrectMaterial;
    public Material correctMaterial;

    // Store the default AnimatorController for the player
    private RuntimeAnimatorController defaultPlayerAnimatorController;

    public SpotlightMover spotlightMover;

    private bool hasDanceMoveStarted = false;

    void Start()
    {
        // Store the default PlayerAnimatorController at the start
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
                    Debug.Log("You pressed number: " + (i + 1));

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

        // get the Animator component from the selected banana man
        Animator bananaManAnimator = mappedObjects[objectIndex].GetComponent<Animator>();

        if (bananaManAnimator != null)
        {
            Animator playerAnimator = playerModel.GetComponent<Animator>();

            playerAnimator.applyRootMotion = false;

            if (playerAnimator != null)
                playerAnimator.runtimeAnimatorController = bananaManAnimator.runtimeAnimatorController;

            if (playerAnimator.runtimeAnimatorController.name == "anim.headspin")
                playerAnimator.applyRootMotion = true;
            if (playerAnimator.runtimeAnimatorController.name == "anim.maraschino")
                playerAnimator.applyRootMotion = true;
        }
    }

    // reset the player's animation to its default controller
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