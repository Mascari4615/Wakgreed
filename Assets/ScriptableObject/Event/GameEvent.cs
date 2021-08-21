using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "GameSystem/GameEvent")]
public class GameEvent : ScriptableObject
{
    [System.NonSerialized] private List<GameEventListener> listeners = new();
    private event Action Collback;
    private event Action<Transform> _Collback;

    public void Raise()
    {
        // Debug.Log($"{name} : Raise");
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            // Debug.Log($"{name} : for {i}");
            listeners[i].OnEventRaised();
        }
        if (!(Collback is null)) Collback.Invoke();
    }

    public void Raise(Transform transform)
    {
         //Debug.Log($"{name} : Raise");
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            //Debug.Log($"{name} : for {i}");
            listeners[i].OnEventRaised();
        }
        if (!(_Collback is null))
        {
           // Debug.Log($"collback");
            //Action<Transform> asdf = (Action<Transform>)Collback.Clone();
            //asdf.Invoke(transform);
            _Collback.Invoke(transform);
           // Debug.Log("µÇ³ª?");
        }
    }

    public void RegisterListener(GameEventListener listener) {listeners.Add(listener);}
    public void UnregisterListener(GameEventListener listener) {listeners.Remove(listener);}
    public void AddCollback(Action a)
    {
        Collback += a;
        //Debug.Log("Its Added");
    }
    public void AddCollback(Action<Transform> a)
    {
        _Collback += a;
        //Debug.Log("Its Added");
    }
    public void RemoveCollback(Action a)
    {
        Collback -= a;
        //Debug.Log("Its Removed");
    }
    public void RemoveCollback(Action<Transform> a)
    {
        _Collback -= a;
        //Debug.Log("Its Removed");
    }
}
