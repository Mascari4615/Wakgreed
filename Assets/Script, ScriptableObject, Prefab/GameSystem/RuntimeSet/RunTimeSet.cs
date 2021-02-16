using System.Collections.Generic;
using UnityEngine;

public abstract class RunTimeSet<T> : ScriptableObject
{
    [System.NonSerialized] public List<T> Items = new List<T>();
    
    public virtual void Add(T t)
    {
        if (!Items.Contains(t)) Items.Add(t);
    }
    
    public virtual void Remove(T t)
    {
        if (Items.Contains(t)) Items.Remove(t);
    }
}