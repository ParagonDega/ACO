using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float offset = 2.0f;

    void Start()
    {
        prefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            prefab.transform.position = hit.point + hit.normal * offset;

            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(prefab, prefab.transform.position, Quaternion.identity);
            }
        }
    }
}
