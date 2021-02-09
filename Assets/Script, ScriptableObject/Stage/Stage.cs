using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Variable/Stage" , order = 3)]
public class Stage : ScriptableObject
{
    public int id;
    public new string name = "저장된 StageName Data 없음";
    public string comment = "저장된 StageComment Data 없음";
    public Room[] roomDatas;
}