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
    public float[] skillCoolDown;
    public AudioClip[] basicAttackAudioClips;
    public AudioClip[] skill0AudioClips;
    public AudioClip[] skill1AudioClips;
    public AudioClip[] skill2AudioClips;
    public TravellerAbilities abilities;
    public Sprite weaponSprite;
}
