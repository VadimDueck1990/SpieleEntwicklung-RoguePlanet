using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionRadius;
    public float explosionForce;
    public float upModifier;
    // public LayerMask interactionLayer;
    // public LayerMask buildLayer;
    public GameObject explosionPrefab;
    // public GameObject jetPrefab;
    // public GameObject jetStream;

    private GameObject explosion;
    private Collider[] bColliders;
    public static int explosionCount;
    // Start is called before the first frame update
    void Start()
    {
        //Invoke("explode", delay);
        Invoke("kill", 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "npc")
        {
            Debug.Log("NPC getroffen");
            NPCMove npcScript = collision.gameObject.GetComponent<NPCMove>();
            if (npcScript != null)
            {
                npcScript.ExplodeAndKill(false);
                explode();
            }
        }

        if(collision.collider.tag == "env")
        {
            Debug.Log("Umgebung getroffen");
            explode();
        }
    }








    // Update is called once per frame
    void explode()
    {
        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, interactionLayer);
        //foreach (Collider collider in hitColliders)
        //{
        //    Rigidbody rb = collider.GetComponent<Rigidbody>();
        //    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upModifier);
        //}

        //Collider[] buildingColliders = Physics.OverlapSphere(transform.position, explosionRadius, buildLayer);
        //foreach (Collider collider in buildingColliders)
        //{
        //    GameObject go = collider.gameObject;
        //    explosion = Instantiate(explosionPrefab, go.transform.position, Quaternion.identity);
        //    go.GetComponent<MeshRenderer>().enabled = false;
        //    go.GetComponent<Collider>().enabled = false;
        //}
        //bColliders = buildingColliders;

        GetComponent<AudioSource>().Play();
        explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;

        Invoke("kill", 5f);
        //Destroy(gameObject);
    }

    void kill()
    {
        //foreach (Collider collider in bColliders)
        //{
        //    Destroy(collider.gameObject);
        //}
        Destroy(explosion);
        Destroy(gameObject);
    }
}
