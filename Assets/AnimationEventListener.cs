using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventListener : MonoBehaviour
{
    [SerializeField] private AnimationEvents[] events;
    private int indexA = 0, indexB = 0;

    public void SetIndex(int index)
    {
        this.indexA = index;
        indexB = 0;
    }

    public void EventCall()
    {
        events[indexA].events[indexB++].Invoke();
    }
}

[System.Serializable]
public class AnimationEvents
{
    public UnityEvent[] events;
}
