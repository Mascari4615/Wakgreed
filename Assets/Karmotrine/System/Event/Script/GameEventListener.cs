using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent Event;
    public UnityEvent Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        // Debug.Log($"{name} : OnEventRaised");
        Response.Invoke();
        // Debug.Log($"{name} : OnEventRaisedEnd");
    }
}