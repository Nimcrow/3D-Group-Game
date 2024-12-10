using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIndicator : MonoBehaviour
{
    public Material bossNormal, bossStunned, bossEnraged; // Materials for the boss
    public SportsBoss bossController;

    private Renderer bossRenderer; // Used to change the boss's material

    // Start is called before the first frame update
    void Start()
    {
        bossRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (bossController.bossState)
        {
            case "Charging":
                if (bossController.isEnraged == false)
                {
                    bossRenderer.material = bossNormal;
                }
                else if (bossController.isEnraged == true)
                {
                    bossRenderer.material = bossEnraged;
                }
                break;
            case "Stunned":
                bossRenderer.material = bossStunned;
                break;
            case "Cooldown":
                if (bossController.isEnraged == false)
                {
                    bossRenderer.material = bossNormal;
                }
                else if (bossController.isEnraged == true)
                {
                    bossRenderer.material = bossEnraged;
                }
                break;

        }
    }
}
