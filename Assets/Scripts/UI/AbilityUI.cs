using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class AbilityUI : MonoBehaviour {
    const int MAX_RENDER_COUNT = 10;
    [SerializeField] private AbilitySummary[] summaryList = new AbilitySummary[MAX_RENDER_COUNT];

    public void Redraw() {
        CmpLinkedList.IntegratedList<Ability> list = AbilityManager.instance.playersAbilities;
        for(int i=0; i<summaryList.Length; i++) {
            if(i < list.Count) {
                summaryList[i].SetView(list[i]);
                summaryList[i].gameObject.SetActive(true);
            } else {
                summaryList[i].gameObject.SetActive(false);
            }
        }
    }
}
