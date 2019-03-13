using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodHoney : MonoBehaviour
{
    public int food = 100;

    private void Update()
    {
        if(food <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ChangeFood(int change)
    {
        food += change;
    }
}
