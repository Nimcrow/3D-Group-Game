using System;
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

    private void Start()
    {
        // ensure only the main camera is enabled at the start
        if (mainCamera != null)
            mainCamera.enabled = true;

        if (danceCamera != null)
            danceCamera.enabled = false;
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

    public void SwitchToDanceCamera()
    {
        if (mainCamera != null)
            mainCamera.enabled = false;

        if (danceCamera != null)
            danceCamera.enabled = true;
    }

    public void SwitchBackToMainCamera()
    {
        if (danceCamera != null)
            danceCamera.enabled = false;

        if (mainCamera != null)
            mainCamera.enabled = true;
    }
}
