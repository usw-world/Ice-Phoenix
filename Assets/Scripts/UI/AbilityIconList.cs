using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;
using CmpLinkedList;

public class AbilityIconList : MonoBehaviour {
    const int MAX_RENDER_COUNT = 10;
    public GameObject[] icons = new GameObject[MAX_RENDER_COUNT];
    
    public void Redraw() {
        IntegratedList<Ability> abilities = AbilityManager.instance.playersAbilities;
        
        for(int i=0; i<icons.Length; i++) {
            if(i<abilities.Count) {
                icons[i].SetActive(true);
                icons[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(60 * i, 0, 0);
                icons[i].GetComponent<AbilityIcon>().SetView(abilities[i]);
            } else {
                icons[i].SetActive(false);
            }
        }
    }
}
