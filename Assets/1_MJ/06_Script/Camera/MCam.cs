using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCam : MonoBehaviour
{
    public GameObject target;

    public float offsetX = 0.0f;
    public float offsetY = 0.0f;
    public float offsetZ = 0.0f;

    Vector3 targetPos;


    void FixedUpdate()
    {
        targetPos = new Vector3(target.transform.position.x + offsetX, target.transform.position.y + offsetY, target.transform.position.z + offsetZ);

        transform.position = targetPos;
    }
}
