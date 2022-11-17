using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _test : MonoBehaviour
{
    ChangeableList<double> doubleList;
    void Start()
    {
        // doubleList = new ChangeableList<double>(1);
        doubleList.SetConditionFunc(ConditionFunc);

        doubleList.AddAfterCheck(2);
        doubleList.AddAfterCheck(3);
        doubleList.AddAfterCheck(4);

        
    }
    public bool ConditionFunc(double tmpNum)
    {
        if (doubleList.count == 0 || tmpNum > doubleList[doubleList.count - 1])
        {
            return true;
        }
        return false;
    }
}
