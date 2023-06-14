using UnityEngine;
using UnityEngine.Serialization;

public abstract class DataBuffer<T> : ScriptableObject
{
    [FormerlySerializedAs("Items")] public T[] items;
}