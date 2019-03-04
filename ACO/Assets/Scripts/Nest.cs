using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : MonoBehaviour
{
    int food = 100;
    const float minFoodUpdateTime = 10.0f;

    private void Start()
    {
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
            food--;
            yield return new WaitForSeconds(minFoodUpdateTime);

        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    void SetFood(int change)
    {
        food += change;
    }
}
