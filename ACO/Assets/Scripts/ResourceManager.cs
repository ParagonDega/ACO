using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{

    public enum ResourceTypes { Honey}
    public ResourceTypes resourceTypes;
    private Node position;
    public Grid grid;

    public float availableResource;
    // Start is called before the first frame update
    void Awake()
    {
    }

    public float ResourceGather(float change)
    {
        float gathered = 0.0f;
        if (-change <= availableResource)
        {
            availableResource += change;
            gathered = -change;
        }
        else
        {
            gathered = availableResource;
            availableResource = 0.0f;

        }
        return gathered;
    }

    // Update is called once per frame
    void Update()
    {
        if (availableResource <= 0)
        {
            Destroy(gameObject);
        }
    }


}
