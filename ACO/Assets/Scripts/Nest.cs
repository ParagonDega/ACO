using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : MonoBehaviour
{
    private const int initFood = 1000;
    public int food;
    const float minFoodUpdateTime = 10.0f;

    private void Start()
    {
        food = initFood;
        StartCoroutine(UpdateNest());
    }

    IEnumerator UpdateNest()
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        while (true)
        {
            if (food > 0)
            {
                food--;
            }
            yield return new WaitForSeconds(minFoodUpdateTime);
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public int ChangeFood(int change)
    {
        int returnFood = 0;
        if (food <= 0)
        {
            food = 0;
        }
        else
        {
            int check = (food + change);
            if (food <= 0 || check <= 0)
            {  //If no food or change will reduce food to 0 or less than 0

                returnFood = change + check;
                food = 0;
            }
            else
            {
                returnFood = change;
                food += change;
            }
        }
        return returnFood;
    }

    public bool FoodLow()
    {
        return food <= initFood / 10;
    }
}
