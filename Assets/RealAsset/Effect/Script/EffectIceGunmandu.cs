using UnityEngine;

[CreateAssetMenu(fileName = "EffectIceGunmandu", menuName = "Effect/IceGunmandu")]
public class EffectIceGunmandu : Effect
{
    [SerializeField] private GameObject iceAura;
    private GameObject instance;

    public override void _Effect()
    {
        instance = Instantiate(iceAura, Wakgood.Instance.transform);
    }

    public override void Return()
    {
        Destroy(instance);
    }
}
