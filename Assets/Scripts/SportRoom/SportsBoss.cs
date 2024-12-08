using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SportsBoss : MonoBehaviour
{
    // Player variables for simplicity
    public GameObject playerObject; // Reference to the player object to get the position of the player
    public int playerHP;


    public int maxHealth; // Max boss health
    public float chargeSpeed; // Speed at which the boss is charging
    public float chargeCd; // Cooldown for the boss between charges
    public float stunTime; // How long the boss is stunned
    public float chargeDuration; // How long the boss charges
    public float idleTimer; // Timer to allow the player to recover after a hit
    public Material bossNormal, bossStunned, bossEnraged; // Materials for the boss

    private Vector3 chargeDir; // The direction the boss is charging
    private int health; // Current boss health
    private string bossState; // The current state that the boss is in
    private Rigidbody rigidBody; // Helps for checking collisions
    private bool isVulnerable; // Allows boss to be damaged
    private float timer; // Game timer
    private FirstPersonController playerController;
    private Renderer bossRenderer;
    private bool isEnraged;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Boss Initalization");
        bossRenderer = GetComponent<Renderer>();
        playerController = playerObject.GetComponent<FirstPersonController>();
        chargeDir = playerController.GetCharacterPosition();
        rigidBody = GetComponent<Rigidbody>();
        isVulnerable = false;
        health = maxHealth;
        isEnraged = false;
        bossState = "Cooldown";
    }

    // Update is called once per frame
    void Update()
    {
        chargeDir.y = 4f;
        // Different switch cases for the states that the boss is in
        switch (bossState)
        {
            case "Charging":
                Charge();
                break;
            case "Stunned":
                Stun();
                break;
            case "Cooldown":
                Cooldown();
                break;

        }

        // Checks for game over
        if (playerHP == 0 || health == 0)
        {
            SceneManager.LoadScene("SportRoom");
        }

    }

    // The boss is stunned allowing the player to attack it
    void Stun()
    {
        Debug.Log("Boss is Stunned");
        bossRenderer.material = bossStunned;
        isVulnerable = true;
        timer += Time.deltaTime;

        if (timer >= stunTime)
        {
            timer = 0f;
            isVulnerable = false;
            bossState = "Cooldown";
        }
    }

    // The boss is actively moving and can hit the player
    void Charge()
    {
        Debug.Log("Boss is charging");
        this.transform.LookAt(chargeDir);
        this.transform.position = Vector3.MoveTowards(this.transform.position, chargeDir, chargeSpeed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= chargeDuration)
        {
            timer = 0f;
            chargeDir = playerController.GetCharacterPosition();
            bossState = "Cooldown";
            Debug.Log(chargeDir);
        }
    }

    // Cooldown between the times that the boss is charging
    void Cooldown()
    {
        Debug.Log("Boss is on cooldown");
        if(isEnraged == false)
        {
            bossRenderer.material = bossNormal;
        }
        else if (isEnraged == true)
        {
            bossRenderer.material = bossEnraged;
        }
        
        timer += Time.deltaTime;

        if (timer >= chargeCd)
        {
            timer = 0f;
            chargeDir = (playerController.GetCharacterPosition() + (playerController.GetCharacterPosition() - this.transform.position));
            bossState = "Charging";
        }
    }

    // Implement later, at 50 or 25 % hp the boss enrages and all of his cooldown timers get shorter making the boss fight harder
    void Enrage()
    {

        isEnraged = true;
        chargeSpeed += 5;
        chargeCd -= 0.5f;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (bossState == "Charging")
        {

            if (coll.gameObject.CompareTag("Walls") || coll.gameObject.CompareTag("Destructible Walls"))
            {
                Debug.Log("Boss Hit a Wall");
                health--;
                if(coll.gameObject.CompareTag("Destructible Walls"))
                {
                    Destroy(coll.gameObject);
                }

                // Checks hp for the enrage mechanic
                if (health <= (maxHealth / 4))
                {
                    Enrage();
                }

                timer = 0f;
                rigidBody.velocity = Vector3.zero;
                chargeDir = (playerController.GetCharacterPosition() + (playerController.GetCharacterPosition() - this.transform.position));
                bossState = "Stunned";
            }
            else if (coll.gameObject.CompareTag("Player"))
            {
                Debug.Log("Boss Hit a Player");
                playerHP--;
                timer = 0f;
                if (timer >= idleTimer)
                {
                    this.transform.position = this.transform.position;
                    chargeDir = (playerController.GetCharacterPosition() + (playerController.GetCharacterPosition() - this.transform.position));
                    bossState = "Cooldown";
                }
                // Implement functionality for damaging player
            }
        }
    }
}