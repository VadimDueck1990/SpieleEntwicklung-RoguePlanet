using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject rocketHand;
    public GameObject flThrowerHand;
    public GameObject rocketPrefab;
    public GameObject damageCollider;
    public GameObject RocketBar;
    public ParticleSystem fireParticles;
    public Material converted;
    public float rocketImpulse;
    public float maxDistance;
    public LayerMask interactionLayer;
    public Camera cam;
    public bool isControlActive;
    public float rocketDelay;
    public float hypnoDelay;

    private bool isRocketEnabled;
    private bool isHypnoEnabled;
    private Healthbar rocketBar;

    // Start is called before the first frame update
    void Start()
    {
        rocketBar = RocketBar.GetComponent<Healthbar>();
        isRocketEnabled = true;
        isHypnoEnabled = true;
        fireParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // firing the rocket
        if (Input.GetKeyDown(KeyCode.Mouse1) && isRocketEnabled)
        {
            FireRocket();
            isRocketEnabled = false;
            StartCoroutine(RocketDelay());
        }
        // firing the flamethrower
        if (Input.GetKey(KeyCode.Mouse0))
        {
            FireFlamethrower();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (fireParticles.isPlaying)
            {
                fireParticles.Stop();
                damageCollider.SetActive(false);
            }
        }

        // firing the rocket
        if (Input.GetKeyDown(KeyCode.E) && isHypnoEnabled)
        {
            HypnotizeTarget();
            isHypnoEnabled = false;
            StartCoroutine(HypnotizationDelay());
        }
    }

    private void FireFlamethrower()
    {
        if (!fireParticles.isPlaying)
        {
            ActivateFlameDamage();
            fireParticles.Play();
            
        }

    }

    private void ActivateFlameDamage()
    {
        damageCollider.SetActive(true);
    }

    private void HypnotizeTarget()
    {
        // get the middle of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // get the target and hypnotize
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, 1000.0f))
        {
            if (rayHit.collider.tag == "npc")
            {
                NPCMove target = rayHit.collider.GetComponent<NPCMove>();
                if (target == null)
                    Debug.Log("hypnotize target is empty");
                target.Hypnotize();
            }
        }
    }

    IEnumerator HypnotizationDelay()
    {
        yield return new WaitForSeconds(hypnoDelay);
        isHypnoEnabled = true;
    }

    private void FireRocket()
    {
        // get the middle of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        GameObject rocket = Instantiate(rocketPrefab, rocketHand.transform.position, rocketHand.transform.rotation) as GameObject;
        Rigidbody rocketRb = rocket.GetComponent<Rigidbody>();
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 1000.0f))
        {
            Debug.DrawLine(rocketHand.transform.position, rayHit.point, Color.red);

            rocketRb.AddForce((rayHit.point - rocketRb.transform.position).normalized * rocketImpulse, ForceMode.Impulse);
        }
        else
        {
            rocketRb.AddForce(rocketHand.transform.position * rocketImpulse, ForceMode.Impulse);
        }
    }

    IEnumerator RocketDelay()
    {
        rocketBar.health = 0;
        yield return new WaitForSeconds(rocketDelay);
        isRocketEnabled = true;
    }
}
