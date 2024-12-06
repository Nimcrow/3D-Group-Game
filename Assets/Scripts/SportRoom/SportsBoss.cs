using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SportsBoss : MonoBehaviour
{
    // Variables for the boss
    public int maxHealth;
    public int health;
    public float chargeSpeed;
    public float chargeCd;
    public float stunTime;

    private Vector3 chargeDir;
    private string bossState;
    private Rigidbody rigidBody;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(bossState) 
        {
            case "Charging":
                Charge();
                break;
            case "Stunned":
                Stun();
                break;
            case "Cooldown":

                break;

        }
    }

    void Stun()
    {

    }

    void Charge()
    {

    }

    private void OnCollisionEnter() 
    {

    }
}
