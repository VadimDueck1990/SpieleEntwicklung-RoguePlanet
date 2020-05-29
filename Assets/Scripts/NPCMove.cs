using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMove : MonoBehaviour
{
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private LayerMask npcLayer;
    [SerializeField]
    private GameObject particlePrefab;
    [SerializeField]
    private GameObject moneyStackPrefab;
    [SerializeField]
    private float attentionRadius;
    [SerializeField]
    private float attackRadius;
    [SerializeField]
    private float wanderRadius;
    [SerializeField]
    private float maxWanderTime;
    [SerializeField]
    private int interval;
    [SerializeField]
    private int health;
    [SerializeField]
    private int maxHealth;

    [SerializeField]
    private Material normal;
    [SerializeField]
    private Material converted;

    private NavMeshAgent navMeshAgent;
    private Transform destination;
    private LayerMask activeLayer;
    private GameObject explosionEffect;
    private GameObject loot;
    private bool isDead;
    private float wanderTimer;


    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<MeshRenderer>().material = normal;
        health = maxHealth;
        activeLayer = playerLayer;
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
            Debug.LogError("The nav mesh agent is not attached to " + gameObject.name);
        wanderTimer = UnityEngine.Random.Range(0f, 5f);
    }


    // Update is called once per frame
    void Update()
    {
        // if target is already set start persecution
        if (destination != null)
        {
            navMeshAgent.SetDestination(destination.transform.position);
        }
        WanderRandomly();

        // check every given frame, if player is nearby
        if (Time.frameCount % interval == 0)
            CheckForPlayerAndSetDestination();
    }

    // Once in a while this method gets called to look for the player
    // and trigger the navMeshAgent
    // and if the player is close enough trigger attack
    void CheckForPlayerAndSetDestination()
    {
        if (destination == null)
        {
            Collider[] col = Physics.OverlapSphere(transform.position, attentionRadius, activeLayer);
            if (col.Length < 1)
                return;

            foreach (Collider collider in col)
            {
                if (collider.transform.position != transform.position)
                {
                    destination = collider.gameObject.transform;
                    Debug.Log("The nav mesh agent is attached to " + collider.gameObject.name);
                }
            }
        }
        else
        {
            Collider[] col = Physics.OverlapSphere(transform.position, attackRadius, activeLayer);
            if (col.Length < 1)
                return;
            if (activeLayer == npcLayer)
            {
                foreach (Collider collider in col)
                {
                    if (collider.transform.position != transform.position)
                    {
                        float distance = Vector3.Distance(collider.transform.position, transform.position);
                        Debug.Log("You have to die: " + collider.gameObject.name);
                        Debug.Log("Distance between NPCs: " + distance);
                        Destroy(collider.gameObject);
                        ExplodeAndKill(false);
                    }
                }
            }
            else
            {
                ExplodeAndKill(true);
            }
        }
    }

    public void Hypnotize()
    {
        destination = null;
        activeLayer = npcLayer;
        Debug.Log("AAAAAAAhhhhh Nooooo!!!! I'm loco!");
    }

    public void ExplodeAndKill(bool isTargetPlayer)
    {
        
        // TODO:
        // some count for the gamemechanic
        if (!isDead)
        {
            if (!isTargetPlayer)
            {
                GameManagement.instance.InstantiateMoney(transform.position);
            }
            else
            {
                GameManagement.instance.RemoveMoney();
            }
            GetComponent<AudioSource>().Play();
            explosionEffect = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            //gameObject.GetComponent<Renderer>().enabled = false;
            MeshRenderer[] allChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer component in allChildren)
            {
                component.enabled = false;
            }
            gameObject.GetComponent<Collider>().enabled = false;
            Invoke("Kill", 5f);
            isDead = true;
        }
    }

    void Kill()
    {
        Destroy(explosionEffect);
        Destroy(gameObject);
    }

    void WanderRandomly()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= maxWanderTime)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
            navMeshAgent.SetDestination(newPos);
            wanderTimer = 0;
        }
    }

    // generates a new random position as target for the AI
    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);

        return navHit.position;
    }

    public void ApplyBurn(int damage)
    {
        print("auauauauaua!!!");
        health -= damage;
        if(health <= 0)
        {
            ExplodeAndKill(false);
        }
    }
}
