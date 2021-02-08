using UnityEngine;

[CreateAssetMenu]
public class Traveller : ScriptableObject
{
    public new string name;
    public int baseHP;
    public int baseAD;
    public float baseAS;
    public int baseCriticalChance;
    public float baseMoveSpeed;
    public float abillity0CoolDown;
    public float abillity1CoolDown;
    public float abillity2CoolDown;
    public AudioClip[] basicAttackAudioClips;
    public AudioClip[] abillity0AudioClips;
    public AudioClip[] abillity1AudioClips;
    public AudioClip[] abillity2AudioClips;
    public TravellerFunctions travellerFunctions;
    public Sprite weaponSprite;
}
