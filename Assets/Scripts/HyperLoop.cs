using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class HyperLoop : MonoBehaviour
{
    public GameObject character;
    public Camera fps;
    public Camera topDown;
    public Camera[] teleportCameras;
    public GameObject teleBar;

    private bool isEnabled = true;
    private float yOffset;
    private bool isTeleportEnabled = true;
    public float teleportDelay;
    private Healthbar bar;

    // Start is called before the first frame update
    void Start()
    {
        bar = teleBar.GetComponent<Healthbar>();
        // get the size of the Character
        Collider m_Collider = character.GetComponent<Collider>();
        yOffset = m_Collider.bounds.extents.y;

        // deactivate top down
        foreach (Camera camera in teleportCameras)
            camera.enabled = false;
        topDown = teleportCameras[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchViewMode();
        }

        // if the player is in teleport mode
        if (topDown.isActiveAndEnabled == true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                foreach (Camera camera in teleportCameras)
                    camera.enabled = false;
                topDown = teleportCameras[0];
                topDown.enabled = true;
                print("Top kamera 1");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                foreach (Camera camera in teleportCameras)
                    camera.enabled = false;
                topDown = teleportCameras[1];
                topDown.enabled = true;
                print("Top kamera 2");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                foreach (Camera camera in teleportCameras)
                    camera.enabled = false;
                topDown = teleportCameras[2];
                topDown.enabled = true;
                print("Top kamera 3");
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                foreach (Camera camera in teleportCameras)
                    camera.enabled = false;
                topDown = teleportCameras[3];
                topDown.enabled = true;
                print("Top kamera 4");
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (isTeleportEnabled)
                {

                    // get the hit point
                    // teleport the player to the position
                    // and set the mode back to fps
                    RaycastHit hit;
                    Ray ray = topDown.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        Vector3 newPos = new Vector3(hit.point.x,
                            hit.point.y + yOffset,
                            hit.point.z);
                        character.transform.position = newPos;
                        SwitchViewMode();
                        isTeleportEnabled = false;
                        StartCoroutine(Teleport());
                    }
                }
            }
        }
    }

    IEnumerator Teleport()
    {
        bar.health = 0;
        yield return new WaitForSeconds(teleportDelay);
        isTeleportEnabled = true;
    }

    // activates the cursor in teleport mode
    void OnGUI()
    {
        if (!isEnabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // switches the cameras and deactivates player controls
    void SwitchViewMode()
    {
        topDown.enabled = isEnabled;
        character.SetActive(!isEnabled);
        fps.enabled = !isEnabled;
        isEnabled = !isEnabled;
    }
}
