using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //Lol singleton
    public static SpawnManager spawnManager;

    private void Awake()
    {
        if(spawnManager != null)
        {
            Debug.LogError("More than one spawn manager in scene!");
        }
        spawnManager = this;
    }

    private GameObject foodToSpawn;
    public GameObject foodHoney;

    private void Start()
    {
        foodToSpawn = foodHoney;
    }

    public GameObject GetFoodToSpawn()
    {
        return foodToSpawn;
    }
}
