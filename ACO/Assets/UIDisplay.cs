using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplay : MonoBehaviour
{
    public Nest nest;
    public Unit worker;
    public Text energyText, unitText, spawnButtonText;

    // Update is called once per frame
    void Update()
    {
        energyText.text = "Energy: " + nest.food;
        unitText.text = "Workers: " + worker.numberOfAnts;
        spawnButtonText.text = "Spawn Worker: " + worker.spawnCost + " energy";
    }

    public void HandleClickQuit()
    {
        //Gather nest data here 
        Debug.Log("Quit");
        StartCoroutine("Quit");
    }

    IEnumerator Quit()
    {
        yield return new WaitForSeconds(10);
        Application.Quit();
    }

}
