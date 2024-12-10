using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events; // universal trigger component

/*
When player walks into spotlight,
play music and switch to danceCamera (3rd person view)
*/

public class SpotlightTrigger : MonoBehaviour
{
    [SerializeField] string tagFilter; // ensure only certain tagged items can invoke
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;

    public Camera mainCamera;
    public Camera danceCamera;
    public Camera audienceCamera_left;
    public Camera audienceCamera_middle;
    public Camera audienceCamera_right;

    public SpotlightMover spotlightMover;

    private void Start()
    {
        mainCamera.enabled = true; // ensure only the main camera is enabled at the start
        danceCamera.enabled = false;
        audienceCamera_left.enabled = false;
        audienceCamera_middle.enabled = false;
        audienceCamera_right.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!String.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        onTriggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!String.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        onTriggerExit.Invoke();
    }

    public void SwitchToMainCamera()
    {
        mainCamera.enabled = true;
        danceCamera.enabled = false;
        audienceCamera_left.enabled = false;
        audienceCamera_middle.enabled = false;
        audienceCamera_right.enabled = false;
    }

    public void SwitchToAudienceCamera()
    {
        if (spotlightMover.leftPositionCam)
            SwitchToLeftCamera();
        if (spotlightMover.middlePositionCam)
            SwitchToMiddleCamera();
        if (spotlightMover.rightPositionCam)
            SwitchToRightCamera();
    }

    // changing to audience view
    private void SwitchToLeftCamera()
    {
        mainCamera.enabled = false;
        danceCamera.enabled = false;
        audienceCamera_left.enabled = true;
        audienceCamera_middle.enabled = false;
        audienceCamera_right.enabled = false;
    }

    private void SwitchToMiddleCamera()
    {
        mainCamera.enabled = false;
        danceCamera.enabled = false;
        audienceCamera_left.enabled = false;
        audienceCamera_middle.enabled = true;
        audienceCamera_right.enabled = false;
    }

    private void SwitchToRightCamera()
    {
        mainCamera.enabled = false;
        danceCamera.enabled = false;
        audienceCamera_left.enabled = false;
        audienceCamera_middle.enabled = false;
        audienceCamera_right.enabled = true;
    }

    private void SwitchToDanceCamera()
    {
        mainCamera.enabled = false;
        danceCamera.enabled = true;
        audienceCamera_left.enabled = false;
        audienceCamera_middle.enabled = false;
        audienceCamera_right.enabled = false;
    }
}
