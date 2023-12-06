using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    Collider collider;
    GameObject[] doors = new GameObject[2];
    bool isOpen;

    Vector3 posDoorLeftOrigin;
    Vector3 posDoorRightOrigin;

    void Awake()
    {
        isOpen = false;
        collider = GetComponent<Collider>();
        doors[0] = transform.GetChild(0).gameObject;
        doors[1] = transform.GetChild(1).gameObject;
        posDoorLeftOrigin = doors[0].transform.position;
        posDoorRightOrigin = doors[1].transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isOpen = true;
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isOpen = false;
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 posDoorLeft = doors[0].transform.position;
        Vector3 posDoorRight = doors[1].transform.position;

        float velocity = 2.0f;

        if(isOpen)
        {
            Vector3 newDoorLeft = new Vector3(1.8f, posDoorLeft.y, posDoorLeft.z);
            Vector3 newDoorRight = new Vector3(-6f, posDoorRight.y, posDoorRight.z);

            posDoorLeft   = Vector3.MoveTowards(posDoorLeft, newDoorLeft, 0.1f);
            posDoorRight  = Vector3.MoveTowards(posDoorRight, newDoorRight, 0.1f);
        }
        else
        {
            posDoorLeft   = Vector3.MoveTowards(posDoorLeft, posDoorLeftOrigin, 0.1f);
            posDoorRight  = Vector3.MoveTowards(posDoorRight, posDoorRightOrigin, 0.1f);
        }

        doors[0].transform.position = posDoorLeft;
        doors[1].transform.position = posDoorRight;
    }
}
