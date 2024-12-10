using UnityEngine;

public class RandomAnimationController : MonoBehaviour
{
    public Animator animator;
    public RuntimeAnimatorController[] animationControllers; // array of animation controllers (different dance animations)

    public SpotlightMover spotlightMover;

    void Start()
    {
        gameObject.SetActive(false); // disable entire game object until start
        if(spotlightMover.hasStart)
        {
            RandomDanceMove();
            gameObject.SetActive(true);
        }
    }

    public void RotateBananaMan(GameObject bananaMan)
    {
        bananaMan.transform.localEulerAngles = new Vector3(0, 90, -90); // x y z rotation to face audience
    }

    public void RandomDanceMove()
    {
        int randomIndex = Random.Range(0, animationControllers.Length);
        animator.runtimeAnimatorController = animationControllers[randomIndex];

        animator.applyRootMotion = false;
        if (animator.runtimeAnimatorController.name == "anim.headspin")
            animator.applyRootMotion = true;
        if (animator.runtimeAnimatorController.name == "anim.maraschino")
            animator.applyRootMotion = true;
    }
}