using System;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventListener : MonoBehaviour
{
    [SerializeField] private AnimationEvents[] events;
    private int indexA, indexB;

    public void SetIndex(int index)
    {
        indexA = index;
        indexB = 0;
    }

    public void EventCall()
    {
        events[indexA].events[indexB++].Invoke();
    }
}

[Serializable]
public class AnimationEvents
{
    public UnityEvent[] events;
}