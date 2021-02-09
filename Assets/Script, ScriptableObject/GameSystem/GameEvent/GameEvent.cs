using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "GameSystem/GameEvent", order = 0)]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> listeners = new List<GameEventListener>();
    private event Action Collback;

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();
        if(!(Collback is null)) Collback.Invoke();
    }

    public void RegisterListener(GameEventListener listener){listeners.Add(listener);}
    public void UnregisterListener(GameEventListener listener){listeners.Remove(listener);}
    public void AddCollback(Action a){Collback += a; Debug.Log("Its Added");}
    public void RemoveCollback(Action a){Collback -= a; Debug.Log("Its Removed");}
}
