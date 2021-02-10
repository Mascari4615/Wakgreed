using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
   public GameEvent Event;
   public UnityEvent Response;

   private void OnEnable()
   { Debug.Log($"{name} : OnEnable"); Event.RegisterListener(this); }

   private void OnDisable()
   { Event.UnregisterListener(this); }

   public void OnEventRaised()
   { Response.Invoke(); }
}
