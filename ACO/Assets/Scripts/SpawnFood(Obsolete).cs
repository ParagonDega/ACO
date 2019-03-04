using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    public Transform objectoToPlace;
    public Camera gamecamera;
    public LayerMask mask;

    private void Update()
    {

        Ray ray = gamecamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray,out hitInfo, mask))
        {
            objectoToPlace.position = hitInfo.point;
            objectoToPlace.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal); //Ensures food is placed with correct rotation
        }
        
    }
}
