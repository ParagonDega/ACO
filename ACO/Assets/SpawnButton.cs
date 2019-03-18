using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    public Unit worker;

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleClick()
    {
        worker.SpawnUnit();
    }
}
