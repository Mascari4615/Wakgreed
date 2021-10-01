using UnityEngine;

[CreateAssetMenu(fileName = "EffectSatellite", menuName = "Effect/EffectSatellite")]
public class EffectSatellite : Effect
{
    [SerializeField] private GameObject prefab;
    private GameObject instance;

    public override void _Effect()
    {
        Transform parent = Wakgood.Instance.transform.Find("SatelliteParent");
        if (parent == null)
        {
            parent = new GameObject("SatelliteParent").transform;
            parent.SetPositionAndRotation(Wakgood.Instance.transform.position, Quaternion.identity);
            parent.SetParent(Wakgood.Instance.transform);
            parent.gameObject.AddComponent<BulletRotate>();
        }

        instance = Instantiate(prefab, parent);
        Vector3 localPos = new();
        if (parent.childCount == 1) localPos.Set(0, 1, 0);
        else if (parent.childCount == 2) localPos.Set(0, -1, 0);
        else if (parent.childCount == 3)
        {
            localPos.Set(Mathf.Cos(330 * Mathf.Deg2Rad), Mathf.Sin(330 * Mathf.Deg2Rad), 0);
            parent.GetChild(1).transform.localPosition = new Vector3(Mathf.Cos(210 * Mathf.Deg2Rad), Mathf.Sin(210 * Mathf.Deg2Rad));
        }
        instance.transform.localPosition = localPos * 3;
    }

    public override void Return()
    {
        Destroy(instance);
    }
}
