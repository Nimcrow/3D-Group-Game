using System;
using System.Collections.Generic;
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

    public List<AudioClip> musicClips;

    private AudioSource audioSourceMusic;
    public AudioSource applauseSFX;
    public AudioSource booSFX;

    public RandomAnimationController animationController;

    public bool isPlayerInSpotlight = false;
    private bool firstTime = true;

    private void Start()
    {
        audioSourceMusic = GetComponent<AudioSource>();

        mainCamera.enabled = true; // ensure only the main camera is enabled at the start
        danceCamera.enabled = false;
        audienceCamera_left.enabled = false;
        audienceCamera_middle.enabled = false;
        audienceCamera_right.enabled = false;
    }

    private void Update()
    {
        // continuously check if the player is in the spotlight
        if (isPlayerInSpotlight && !spotlightMover.hasStart 
            && Input.GetKeyDown(KeyCode.E) && firstTime)
        {
            spotlightMover.StartMiniGame();
            spotlightMover.hasStart = true;
            firstTime = false;
            onTriggerEnter.Invoke();
        }

        if (spotlightMover.spotlightSwitches >= 5)
        {
            onTriggerExit.Invoke();
            spotlightMover.StopMiniGame();
            SwitchToMainCamera();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!String.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        isPlayerInSpotlight = true;

        if (spotlightMover.hasStart)
            onTriggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!String.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        isPlayerInSpotlight = false;

        if (spotlightMover.hasStart)
            onTriggerExit.Invoke();
    }

    public void PlayMusic()
    {
        booSFX.Pause();

        string animatorControllerName = animationController.animator.runtimeAnimatorController.name;

        switch (animatorControllerName)
        {
            case "anim.gangnamstyle":
                audioSourceMusic.clip = musicClips[0];
                break;
                
            case "anim.headspin":
                audioSourceMusic.clip = musicClips[1]; 
                break;

            case "anim.maraschino":
                audioSourceMusic.clip = musicClips[2]; 
                break;

            case "anim.rumba":
                audioSourceMusic.clip = musicClips[3];
                break;

            case "anim.shuffling":
                audioSourceMusic.clip = musicClips[4];
                break;

            case "anim.weirdwalk":
                audioSourceMusic.clip = musicClips[5];
                break;
        }

        audioSourceMusic.Play();
        applauseSFX.Play();
    }

    public void PauseMusic()
    {
        audioSourceMusic.Pause();
        applauseSFX.Pause();
        booSFX.Play();
    }

    public void SwitchToMainCamera()
    {
        mainCamera.enabled = true;
        danceCamera.enabled = false;
        audienceCamera_left.enabled = false;
        audienceCamera_middle.enabled = false;
        audienceCamera_right.enabled = false;
    }

    public void SwitchToDanceCamera()
    {
        mainCamera.enabled = false;
        danceCamera.enabled = true;
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

}