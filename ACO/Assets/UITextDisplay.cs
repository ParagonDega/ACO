using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextDisplay : MonoBehaviour
{
    public Nest nest;
    public Unit worker;
    public Text energyText, unitText, spawnButtonText;

    // Update is called once per frame
    void Update()
    {
        energyText.text = "Energy: " + nest.food;
        unitText.text = "Workers: " + worker.numberOfAnts;
        spawnButtonText.text = "Spawn Worker: "+ worker.spawnCost +" energy";
    }
}
