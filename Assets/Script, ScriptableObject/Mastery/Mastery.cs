using UnityEngine;

[CreateAssetMenu(fileName = "Mastery", menuName = "Variable/Mastery")]
public class Mastery : ScriptableObject
{
    public int ID;
    public new string name;
    [TextArea] public string description;
    public string comment;
    public Sprite sprite;
    [SerializeField] private Effect[] effects;
    public void OnEquip()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i]._Effect();
        }
    }
}