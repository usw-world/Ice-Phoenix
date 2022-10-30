using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;
using Cinemachine;

public class CameraTest : MonoBehaviour
{
    public GameObject cam;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            cam.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            cam.SetActive(false);
        }
    }   
}