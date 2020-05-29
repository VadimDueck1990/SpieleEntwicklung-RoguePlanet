using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public string message;
    public GameObject gridPrefab;
    public int gridSize;
    public int maxGridLevel;

    private static GameSettings _instance;
    public static GameSettings Instance { get { return _instance; } }
    // Start is called before the first frame update
    void Awake()
    {
        if(_instance == null)
        _instance = this;
        else if(_instance != this)
        {
            Destroy(gameObject);
        }
        setupGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // place the grid
    void setupGrid()
    {
        for (int i = 0; i < maxGridLevel; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                for (int k = 0; k < gridSize; k++)
                {
                   GameObject grid = Instantiate(gridPrefab, new Vector3(j, i, k), Quaternion.identity);
                   // grid.GetComponent<Collider>().enabled = false;
                }
            }
        }
    }
}
