using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour
{
    public Nest nest;
    public Text energyText;

    // Update is called once per frame
    void Update()
    {
        energyText.text = "Energy: " + nest.food;
    }
}
