using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    public NavMeshAgent agent;

    public GameObject player;
    public Transform playerTransform;

    public LayerMask groundMask, playerMask;

    public Vector3 targetPos;
    public bool targeted; //Whether it has targeted a proper location.

    public float coolDown; 

    public float wanderRange, sightRange, attackRange;
    public bool playerInSight, onCoolDown;

    private void Awake()
    {
        player = GameObject.Find("Player");
        playerTransform = player.transform;
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        //Look for player in its spherical range.
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerMask);

        if (coolDown > 0) CoolDown();
        else if (!playerInSight) Wandering();
        else if (playerInSight) Chasing();
    }

    public void CoolDown()
    {
        agent.SetDestination(transform.position); //stops moving when on cool down.
        coolDown -= Time.deltaTime; 
    }

    public void Wandering()
    {
        //Randomizes a position within navmesh.
        if (!targeted) 
        {   
            float randomZ = Random.Range(-wanderRange, wanderRange);
            float randomX = Random.Range(-wanderRange, wanderRange);
            targetPos = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            //Checks if the position is inside of the navmesh.
            if (Physics.Raycast(targetPos, -transform.up, 2f, groundMask)){
                NavMeshHit hit;
                if(NavMesh.SamplePosition(targetPos, out hit, 1f, NavMesh.AllAreas)){
                    targeted = true;
                }
            }
        }
        
        //Moves towards the position.
        if (targeted)
        {
            agent.SetDestination(targetPos);

            //Goes on cooldown when reaches the position.
            Vector3 distanceToWalkPoint = transform.position - targetPos;
            if (distanceToWalkPoint.magnitude < 1f)
            {
                targeted = false;
                coolDown = Random.Range(0f, 2f);
            }
        }

    }
    
    public void Chasing()
    {
        //Moves towards the player.
        agent.SetDestination(playerTransform.position);
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject == player)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            //knocks player back, and plays effect, if player is not climbing.
            if (!pc.rb.isKinematic){
                pc.rb.AddForce(transform.forward * 10 + Vector3.up * 10, ForceMode.Impulse);
                pc.slamVFX.transform.position = transform.position;
				pc.slamVFX.transform.rotation = Quaternion.LookRotation(transform.forward);
				pc.slamVFX.GetComponent<ParticleSystem>().Play();
            }
            //goes on cooldown, 
            coolDown = Random.Range(1f, 3f);
        }
    }
}
