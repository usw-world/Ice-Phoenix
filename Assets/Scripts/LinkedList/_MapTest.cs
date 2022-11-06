using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class _MapTest : MonoBehaviour
{
    public GameObject[] prefabMaps;

    ChangeableList<GameObject> mapList;
    // Start is called before the first frame update
    void Start()
    {
        mapList = new ChangeableList<GameObject>(prefabMaps[0]); // 생성자는 리스트의 Head를 지정
        mapList.SetConditionFunc(ConditionFunc);
        InitialMapList();
        InitialMapInGame();
    }

    private bool ConditionFunc(GameObject newMap)
    {
        if (newMap.transform.Find("Circle") || newMap.transform.childCount > 1) // 두번째 맵에 Circle이 있고 세번째 맵은 Object가 1개 이상
            return true;

        return false;
    }

    private void InitialMapList()
    {
        for (int i = 0; i < prefabMaps.Length; i++)
        {
            mapList.AddAfterCheck(prefabMaps[i]);
        }
    }

    private void InitialMapInGame()
    {
        GameObject tmpMap = Instantiate(mapList[0]);
        for (int i = 1; i < mapList.count; i++)
        {
            tmpMap = Instantiate(mapList[i], new Vector2(tmpMap.transform.position.x + tmpMap.transform.Find("Ground").GetComponent<Transform>().localScale.x, 0), transform.rotation);
        }
    }
}
