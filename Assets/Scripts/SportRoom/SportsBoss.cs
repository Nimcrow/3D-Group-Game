using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SportsBoss : MonoBehaviour
{
    public GameObject playerObject; // Reference to the player object to get the position of the player

    public int maxHealth; // Max boss health
    public float chargeSpeed; // Speed at which the boss is charging
    public float chargeCd; // Cooldown for the boss between charges
    public float stunTime; // How long the boss is stunned
    public float chargeDuration; // How long the boss charges
    public float idleTimer; // Timer to allow the player to recover after a hit

    private Vector3 chargeDir; // The direction the boss is charging
    private int health; // Current boss health
    private string bossState; // The current state that the boss is in
    private Rigidbody rigidBody; // Helps for checking collisions
    private bool isVulnerable; // Allows boss to be damaged
    private float timer; // Game timer
    private FirstPersonController playerController;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Boss Initalization");
        playerController = playerObject.GetComponent<FirstPersonController>();
        chargeDir = playerController.GetCharacterPosition();
        rigidBody = GetComponent<Rigidbody>();
        isVulnerable = false;
        health = maxHealth;
        bossState = "Cooldown";
    }

    // Update is called once per frame
    void Update()
    {
        chargeDir.y = 4f;
        // Different switch cases for the states that the boss is in
        switch(bossState) 
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

        // Checks hp for the enrage mechanic
        if (health <= (maxHealth / 4))
        {
            Enrage();
        }
    }

    // The boss is stunned allowing the player to attack it
    void Stun()
    {
        Debug.Log("Boss is Stunned");
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
        //rigidBody.MovePosition(transform.position + chargeDir * chargeSpeed * Time.deltaTime);
        this.transform.position = Vector3.MoveTowards(this.transform.position, chargeDir, chargeSpeed * Time.deltaTime);

        timer += Time.deltaTime;
        if(timer >= chargeDuration)
        {
            timer = 0f;
            this.transform.position = this.transform.position;
            //rigidBody.velocity = Vector3.zero;
            chargeDir = playerController.GetCharacterPosition();
            bossState = "Cooldown";
            Debug.Log(chargeDir);
        }
    }

    // Cooldown between the times that the boss is charging
    void Cooldown() 
    {
        Debug.Log("Boss is on cooldown");
        timer += Time.deltaTime;

        if(timer >= chargeCd)
        {
            timer = 0f;
            bossState = "Charging";
        }
    }

    // Implement later, at 50 or 25 % hp the boss enrages and all of his cooldown timers get shorter making the boss fight harder
    void Enrage()
    {

    }

    private void OnCollisionEnter(Collision coll) 
    {
        if (bossState == "Charging") 
        {

            if (coll.gameObject.CompareTag("Walls"))
             {
                Debug.Log("Boss Hit a Wall");
                timer = 0f;
                this.transform.position = this.transform.position;
                //rigidBody.velocity = Vector3.zero;
                bossState = "Stunned";
            }
            else if (coll.gameObject.CompareTag("Player"))
            {
                Debug.Log("Boss Hit a Player");
                timer = 0f;
                if(timer >= idleTimer)
                {
                    this.transform.position = this.transform.position;
                    //rigidBody.velocity = Vector3.zero;
                    bossState = "Cooldown";
                }
                // Implement functionality for damaging player
            }
        }
    }
}
