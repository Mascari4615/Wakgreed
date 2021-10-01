using UnityEngine;

public class PoolingObject : MonoBehaviour
{
    private void OnDisable() { ObjectManager.Instance.PushObject(gameObject); }
}
