using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour
{
    [SerializeField]
    private GameObject moneyPrefab;
    [SerializeField]
    private GameObject npcPrefab;
    [SerializeField]
    private GameObject moneyBarPrefab;
    [SerializeField]
    private GameObject speedBarPrefab;
    [SerializeField]
    private GameObject inGame;
    [SerializeField]
    private GameObject GameOver;
    [SerializeField]
    private GameObject YouWon;

    [SerializeField] private float currentMoney;
    private float maxMoney;
    [SerializeField] private float moneyMultplier;
    [SerializeField] private float maxNpcs;
    [SerializeField] private GameObject playerPrefab;

    public Text gameMessage;
    public RectTransform mPanel;

    public static GameManagement instance;
    private Healthbar moneyBar;
    private Healthbar speedBar;
    private PlayerCharacterController pController;

    private float currentNpcs;
    private float maxSpeed;
    private float minSpeed = 1f;
    private float movespeed;

    private bool stillIngame;
    private bool takeMoney;

    // Start is called before the first frame update
    private void Awake()
    {
        MakeGamemanagement();
    }

    void MakeGamemanagement()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        pController = playerPrefab.GetComponent<PlayerCharacterController>();
        speedBar = speedBarPrefab.GetComponent<Healthbar>();
        moneyBar = moneyBarPrefab.GetComponent<Healthbar>();
        maxMoney = moneyBar.maximumHealth;
        currentMoney = moneyBar.minimumHealth;
        maxSpeed = speedBar.maximumHealth;
        SetupNPCs();
        setPlayerSpeed();
        InvokeRepeating("SetupNPC",5f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfWon();
        
    }

    private void CheckIfWon()
    {
        if (currentMoney >= maxMoney)
        {
            mPanel.gameObject.SetActive(true);
            gameMessage.text = "You Won!";
            Invoke("ReloadScene", 4f);
        }
        if (currentMoney < 0)
        {
            mPanel.gameObject.SetActive(true);
            gameMessage.text = "You Lost!";
            Invoke("ReloadScene", 4f);
        }

        Vector3 pos = playerPrefab.transform.position;
        if (pos.y < -50)
        {
            mPanel.gameObject.SetActive(true);
            gameMessage.text = "You Lost!";
            Invoke("ReloadScene", 4f);
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SetupNPCs()
    {
        for (int i = 0; i < maxNpcs; i++)
        {
            Vector3 pos = GetRandomLocation();
            InstantiateNPC(pos);

        }
    }

    void SetupNPC()
    {
        Vector3 pos = GetRandomLocation();
        InstantiateNPC(pos);
    }

    public void AddMoney()
    {
        currentMoney += moneyMultplier;
        moneyBar.GainHealth(moneyMultplier);
        Debug.Log("current money: " + currentMoney);
    }

    public void RemoveMoney()
    {
        currentMoney -= moneyMultplier;
        moneyBar.TakeDamage((int)moneyMultplier);
        Debug.Log("Removed Money");
    }

    public void setPlayerSpeed()
    {
        float speed = (maxNpcs / 9f) + (float)(maxNpcs - currentNpcs);
        movespeed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        pController.moveSpeed = movespeed;
        speedBar.SetHealth(Mathf.Clamp(speed, minSpeed, maxSpeed));
    }

    public void InstantiateNPC(Vector3 pos)
    {
        Instantiate(npcPrefab, pos, Quaternion.identity);
        setPlayerSpeed();
        currentNpcs++;
        if (movespeed == minSpeed)
        {
            if(takeMoney)
                RemoveMoney();
            takeMoney = true;

        }
        Debug.Log("NPCs auf dem Feld: " + currentNpcs);
    }

    // is called from npc script
    public void InstantiateMoney(Vector3 position)
    {
        currentNpcs--;
        Instantiate(moneyPrefab, position, Quaternion.identity);
        setPlayerSpeed();
        Debug.Log("NPCs auf dem Feld: " + currentNpcs);
    }

    Vector3 GetRandomLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        // Pick the first indice of a random triangle in the nav mesh
        int t = UnityEngine.Random.Range(0, navMeshData.indices.Length - 3);

        // Select a random point on it
        Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], UnityEngine.Random.value);
        Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], UnityEngine.Random.value);

        return point;
    }
}
