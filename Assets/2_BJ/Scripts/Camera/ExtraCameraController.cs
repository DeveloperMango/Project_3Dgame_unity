using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraCameraController : MonoBehaviour
{
    Camera mainCam;
    public Camera extraCam;
    Collider collider;

    void Awake()
    {
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        extraCam = gameObject.GetComponentInChildren<Camera>();
        collider = GetComponent<Collider>();
        mainCam.enabled = true;
        extraCam.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            extraCam.enabled = true;
            mainCam.enabled = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            mainCam.enabled = true;
            extraCam.enabled = false;
        }
    }
}
