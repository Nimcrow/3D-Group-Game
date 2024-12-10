using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private CharacterAnimation enemyAnim;
    private Rigidbody myBody;
    public float speed = 5f;
    private Transform playerTarget;
    public float attack_Distance = 1f;
    private float chase_Player_After_Attack = 1f;
    private float current_Attack_Time;
    private float default_Attack_Time = 2f;
    private bool followPlayer;
    private bool attackPlayer;

    void Awake()
    {
        enemyAnim = GetComponentInChildren<CharacterAnimation>();
        myBody = GetComponent<Rigidbody>();

        playerTarget = GameObject.FindWithTag(Tags.PLAYER_TAG).transform;
    }

    void Start()
    {
        followPlayer = true;
        current_Attack_Time = default_Attack_Time;
    }

    void Update()
    {
        Attack();
    }

    void FixedUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        if (!followPlayer)
        {
            return;
        }

        // Distance check
        float distance = Vector3.Distance(transform.position, playerTarget.position);

        // Restrict to z-axis movement only (ignore x and y axes)
        Vector3 targetPosition = new Vector3(playerTarget.position.y, playerTarget.position.y, playerTarget.position.z);

        if (distance > attack_Distance)
        {
            // Only look and move along the z-axis
            transform.LookAt(playerTarget);

            //// Move only in the z direction (forward) relative to the target
            Vector3 direction = (targetPosition - transform.position).normalized;
            myBody.velocity = new Vector3(playerTarget.position.y, playerTarget.position.y, direction.z * speed);  // Move only on the z-axis
            myBody.velocity = transform.forward * speed;

            if (myBody.velocity.sqrMagnitude != 0)
            {
                enemyAnim.Walk(true);
            }
        }
        else if (distance <= attack_Distance)
        {
            myBody.velocity = Vector3.zero;
            enemyAnim.Walk(false);
            followPlayer = false;
            attackPlayer = true;
        }
    }


    void Attack()
    {
        if (!attackPlayer)
        {
            return;
        }

        current_Attack_Time += Time.deltaTime;

        if (current_Attack_Time > default_Attack_Time)
        {
            enemyAnim.EnemyAttack(Random.Range(0, 3));
            current_Attack_Time = 0f;
        }

        if (Vector3.Distance(transform.position, playerTarget.position) > attack_Distance + chase_Player_After_Attack)
        {
            attackPlayer = false;
            followPlayer = true;
        }
    }
}
