using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WakgoodCollider : MonoBehaviour
{
    private readonly Dictionary<int, InteractiveObject> nearInteractiveObjectDic = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("UpperDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.up, 1));
        else if (other.CompareTag("LowerDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.down, 0));
        else if (other.CompareTag("LeftDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.left, 3));
        else if (other.CompareTag("RightDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.right, 2));
        else if (other.CompareTag("AreaDoor")) GameManager.Instance.ChangeArea(other.transform);
        else if (other.CompareTag("InteractiveObject"))
        {
            if (!nearInteractiveObjectDic.ContainsKey(other.GetInstanceID()))
            {
                nearInteractiveObjectDic.Add(other.GetInstanceID(), other.GetComponent<InteractiveObject>());
            }
            else
            {
                Debug.Log($"왁굳 트리거 엔터 : {other.gameObject.name}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("InteractiveObject"))
            return;

        if (nearInteractiveObjectDic.ContainsKey(other.GetInstanceID()))
        {
            nearInteractiveObjectDic.Remove(other.GetInstanceID());
        }
        else
        {
            Debug.Log($"왁굳 트리거 엑싯 : {other.gameObject.name}");
        }
    }

    public InteractiveObject GetNearestInteractiveObject()
    {
        InteractiveObject nearInteractiveObject = null;
        float distance = float.MaxValue;
        
        foreach (InteractiveObject item in nearInteractiveObjectDic.Values.Where(item => Vector2.Distance(transform.position, item.transform.position) < distance))
        {
            nearInteractiveObject = item;
            distance = Vector2.Distance(transform.position, item.transform.position);
        }

        return nearInteractiveObject;
    }
}
