using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableUI : MonoBehaviour
{
    public SpotlightMover spotlightMover;
    public GameObject spotlightTrigger;

    private bool hasDanceMoveStarted = false;

    void Start()
    {
        // disable everything except the SpotlightTrigger at the start if the mini-game hasn't started
        if (!spotlightMover.hasStart)
            DisableAllExceptSpotlightTrigger();
    }

    void Update()
    {
        if (spotlightMover.hasStart && !hasDanceMoveStarted)
        {
            EnableGameObjectsInChildren();
            hasDanceMoveStarted = true;
        }
    }

    // disable all child GameObjects except the SpotlightTrigger
    public void DisableAllExceptSpotlightTrigger()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject != spotlightTrigger) // Skip disabling the SpotlightTrigger
            {
                child.gameObject.SetActive(false); // Disable the child GameObject
            }
        }
    }

    // enable all child GameObjects
    public void EnableGameObjectsInChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}