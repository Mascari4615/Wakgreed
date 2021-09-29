using UnityEngine;

[CreateAssetMenu(fileName = "EffectSatellite", menuName = "Effect/EffectSatellite")]
public class EffectSatellite : Effect
{
    [SerializeField] private GameObject satellitePrefab;
    private GameObject satelliteInstance;

    public override void _Effect()
    {
        if (!Wakgood.Instance.transform.Find("SatelliteParent"))
        {
            GameObject temp = new("SatelliteParent");
            temp.transform.SetPositionAndRotation(Wakgood.Instance.transform.position, Quaternion.identity);
            temp.transform.SetParent(Wakgood.Instance.transform);
            temp.AddComponent<BulletRotate>();
        }
        
        // satelliteInstance = ObjectManager.Instance.PopObject(satellitePrefab.name, Wakgood.Instance.transform.Find("SatelliteParent"), false, true);
        satelliteInstance = Instantiate(satellitePrefab, Wakgood.Instance.transform.Find("SatelliteParent"));
        satelliteInstance.transform.localPosition = Vector3.up * 3f;
    }

    public override void Return()
    {
        //ObjectManager.Instance.PushObject(Wakgood.Instance.transform.Find("SatelliteParent").Find(satelliteInstance.name).gameObject);
        Destroy(Wakgood.Instance.transform.Find("SatelliteParent").Find(satelliteInstance.name).gameObject);
    }
}
