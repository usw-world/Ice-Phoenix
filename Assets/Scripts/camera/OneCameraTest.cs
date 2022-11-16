using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;
using Cinemachine;

public class OneCamera : MonoBehaviour
{
    public CinemachineConfiner cam;    
    public Collider2D Room1;
    public Collider2D Room2;
    private void Start() {
        cam.GetComponent<CinemachineConfiner>();
        cam.m_BoundingShape2D = Room1;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Room1"))
        {
            cam.m_BoundingShape2D = Room1;
        }
        else if(other.CompareTag("Room2"))
        {
            cam.m_BoundingShape2D = Room2;
        }
    }
    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if(other.CompareTag("Player") && !other.isTrigger)
    //     {
    //         cam.m_BoundingShape2D = Room2;
    //     }
    // }   
}