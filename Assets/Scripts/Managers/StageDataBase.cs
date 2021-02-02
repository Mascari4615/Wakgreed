using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stage
{
    public int id;
    public string name = "저장된 StageName Data 없음";
    public string comment = "저장된 StageComment Data 없음";
    public GameObject prefab;
    public RoomData[] roomDatas;
}

public class StageDataBase : MonoBehaviour
{
    private static StageDataBase instance;
    [HideInInspector] public static StageDataBase Instance { get { return instance; } }

    public Stage[] stages;

    void Awake()
    {
        instance = this;
    }
}