using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // ADD POWER UPS TO SLOW BOSS, SPEED UP PLAYER, STUN BOSS, JUMP BOOST, RESTORE HEALTH AND DOUBLE DAMAGE
    public Material power1, power2, power3;
    public FirstPersonController playerController;
    public SportsBoss bossController;
    public float buffTimer;

    private int powerType;
    private Vector3 pos;
    private Renderer powerRenderer;
    private float timer;
    private bool destroy;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        destroy = false;
        timer = 0f;
        powerType = Random.Range(1, 4);
        pos.y = 2.5f;
        pos.x = Random.Range(-25f, 25f);
        pos.z = Random.Range(-25f, 25f);
        this.transform.position = pos;
        powerRenderer = GetComponent<Renderer>();


        switch (powerType)
        {
            case 1:
                powerRenderer.material = power1;
                break;
            case 2:
                powerRenderer.material = power2;
                break;
            case 3:
                powerRenderer.material = power3;
                break;
        }

    }

    void Update()
    {
        if (destroy == true)
        {
            pos.x = 1000f;
            this.transform.position = pos;
            timer += Time.deltaTime;
            if (timer > buffTimer)
            {
                playerController.SetJumpHeight(8f);
                bossController.chargeSpeed = 20;
                playerController.SetCharacterSpeed(3f, 6f);
                destroy = false;
                if(gameObject.name == "Power Up(Clone)")
                {
                    Destroy(this.gameObject);
                }
                
            }
            
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            audioManager.PlaySFX(audioManager.powerUp);
            switch (powerType)
            {
                case 1:
                    bossController.chargeSpeed = 10;
                    destroy = true;
                    timer = 0f;
                    break;
                case 2:

                    playerController.SetCharacterSpeed(6f, 12f);
                    destroy = true;
                    timer = 0f;
                    break;
                case 3:   
                    playerController.SetJumpHeight(20f);
                    destroy = true;
                    timer = 0f;
                    break;
            }

        }
    }
}
