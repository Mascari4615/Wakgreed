using UnityEngine;

public class PoolingObject : MonoBehaviour
{
    private void OnDisable()
    {
        /*if (gameObject.GetComponent<Monster>())
        {
            Debug.Log($"Disable Push {gameObject.name} : {gameObject.GetInstanceID()}");
        }*/
        ObjectManager.Instance.PushObject(gameObject);
    }
}
