using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMapTest : MonoBehaviour
{
    // public GameObject Player;    
    // public GameObject MainRoom;
    // public GameObject MainRoomPortal;
    // public GameObject MainRoomSpawn;

    // public GameObject FstRoom;
    // public GameObject ScdRoom;
    // public GameObject TrdRoom;

    // public GameObject FstRoomSpawnZone;
    // public GameObject ScdRoomSpawnZone;
    // public GameObject TrdRoomSpawnZone;

    // public GameObject Location1;
    // public GameObject Location2;
    // public GameObject Location3;

    // GameObject[] map = new GameObject[3];
    // int[] arr = new int[3];
    
    // private void Awake()
    // {
    //     map[0] = FstRoom;
    //     map[1] = ScdRoom;
    //     map[2] = TrdRoom;

    //     for (int i = 0; i < map.Length; i++) {
    //         GameObject temp = map[i];
    //         int randomIndex = Random.Range(0, map.Length);
    //         map[i] = map[randomIndex];
    //         map[randomIndex] = temp;
    //     }
    // }

    // void Start()
    // {        
    //     for (int i = 0; i < map.Length; i++) {
    //         print(map[i]);
    //     }

    //     map[0].transform.position = Location1.transform.position;
    //     map[1].transform.position = Location2.transform.position;
    //     map[2].transform.position = Location3.transform.position;

    // }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if(other.tag == "MainPortal")
    //     {
    //         if(map[0] == FstRoom) {
    //             Player.transform.position = FstRoomSpawnZone.transform.position;
    //         } else if(map[0] == ScdRoom) {
    //             Player.transform.position = ScdRoomSpawnZone.transform.position;
    //         } else if(map[0] == TrdRoom) {
    //             Player.transform.position = TrdRoomSpawnZone.transform.position;
    //         }
    //     }
    // }
}