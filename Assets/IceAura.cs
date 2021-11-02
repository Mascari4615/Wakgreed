using UnityEngine;
using UnityEngine.Serialization;

public class IceAura : MonoBehaviour
{
    [FormerlySerializedAs("OnMonsterKill")] [SerializeField] private GameEvent onMonsterKill;
    private int stack;
}
