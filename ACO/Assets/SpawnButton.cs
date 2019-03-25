using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    public Unit worker;

    public void HandleClick()
    {
        worker.SpawnUnit();
    }
}
