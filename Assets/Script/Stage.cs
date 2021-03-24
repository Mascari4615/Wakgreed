using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Variable/Stage")]
public class Stage : ScriptableObject
{
    public int id;
    public new string name;
    public string comment;
    public Room[] roomDatas;
}