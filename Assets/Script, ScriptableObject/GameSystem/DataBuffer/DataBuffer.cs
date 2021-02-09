using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataBuffer<T> : ScriptableObject
{
    public T[] Items;
}
