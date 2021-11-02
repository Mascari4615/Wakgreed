using System.Collections.Generic;
using UnityEngine;

public class WakgoodCollider : MonoBehaviour
{
    public Dictionary<int, InteractiveObject> NearInteractiveObjectDic { get; private set; } = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("UpperDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.up, 1));
        else if (other.CompareTag("LowerDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.down, 0));
        else if (other.CompareTag("LeftDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.left, 3));
        else if (other.CompareTag("RightDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.right, 2));

        else if (other.CompareTag("AreaDoor")) AreaTweener.Instance.ChangeArea(other.transform);

        else if (other.CompareTag("InteractiveObject"))
        {
            if (!NearInteractiveObjectDic.ContainsKey(other.GetInstanceID())) NearInteractiveObjectDic.Add(other.GetInstanceID(), other.GetComponent<InteractiveObject>());
            else Debug.LogError("ㅈ버그");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("InteractiveObject"))
            return;

        if (NearInteractiveObjectDic.ContainsKey(other.GetInstanceID())) NearInteractiveObjectDic.Remove(other.GetInstanceID());
        else Debug.LogError("ㅈ버그");
    }
}
