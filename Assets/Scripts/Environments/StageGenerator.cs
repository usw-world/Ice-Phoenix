using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour {
    [SerializeField] GameObject[] roomList;
    [SerializeField] float interval = 66f;
    [SerializeField] float offsetY = -28.45f;

    void Start() {
        System.Array.Sort(roomList, new Compareable());
        float offsetX = interval/2;
        foreach(GameObject room in roomList) {
            GameObject r = Instantiate(room, this.transform);
            r.transform.position = new Vector3(offsetX, offsetY, 0);
            offsetX += interval;
        }
    }
    class Compareable : IComparer {
        public int Compare(object x, object y){
            try {
                return Random.Range(0, 10) - Random.Range(0, 10);
            } catch {
                return -1;
            }
        }
    }
}