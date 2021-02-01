using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData : MonoBehaviour
{
    [HideInInspector] public RoomData[] roomDatas;
    
    public string stageName = "저장된 StageName Data 없음";
    public string stageComment = "저장된 StageComment Data 없음";

    void Awake()
    {
        roomDatas = new RoomData[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) roomDatas[i] = transform.GetChild(i).GetComponent<RoomData>();
    }
}
