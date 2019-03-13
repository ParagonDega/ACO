using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyDisplay : MonoBehaviour
{
    public int energy = 5;
    public Text energyText;
    public Nest homeNest;
    // Update is called once per frame
    void Update()
    {
        energyText.text = "Energy:" + homeNest.food;
    }
}
