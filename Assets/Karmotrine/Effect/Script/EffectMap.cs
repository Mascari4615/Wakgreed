using UnityEngine;

[CreateAssetMenu(fileName = "EffectMap", menuName = "Effect/Map")]
public class EffectMap : Effect
{
    public override void Return()
    {
    }

    public override void _Effect()
    {
        StageManager.Instance.SeeAllMap();
    }
}