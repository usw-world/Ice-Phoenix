using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using CmpLinkedList;
using Unity.VisualScripting.FullSerializer;

public class ChangeableList<T> : CmpLinkedList.LinkedList<T>, IComparable<T>
{
    protected Func<T, bool> FuncWithContidion;
    
    protected virtual void Start() { }

    public void SetConditionFunc(Func<T, bool> tmpFunc) => FuncWithContidion = tmpFunc;

    public void AddAfterCheck(T item)
    {
        if (FuncWithContidion(item))
        {
            InsertTailNode(item);
        }
    }

    public ChangeableList(Func<T, bool> condition) {
        FuncWithContidion += condition;
        FuncWithContidion += condition;
        FuncWithContidion += condition;
    }

}