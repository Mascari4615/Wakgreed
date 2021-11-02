using System.Collections.Generic;
using UnityEngine;

public abstract class RunTimeSet<T> : ScriptableObject
{
    [System.NonSerialized] public readonly List<T> Items = new();
    
    public virtual void Add(T t)
    {
        // if (!Items.Contains(t))
        {
            Items.Add(t);
        }
    }
    
    public virtual void Remove(T t)
    {
        if (Items.Contains(t))
        {
            Items.Remove(t);
        }
        else
        {
            // Debug.Log("RunTimeSet : �������� �ʴ� ������ ���� �õ�");
        }
    }

    public virtual void Clear()
    {
        if (Items == null) return;

        int count = Items.Count;
        for (int i = 0; i < count; i++)
        {
            Remove(Items[0]);
        }
    }
}