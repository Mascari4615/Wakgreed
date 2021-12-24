using Cinemachine;
using UnityEngine;

public enum AreaType
{
    Normal,
    Restaurant,
    Shop
}

public class AreaTweener : MonoBehaviour
{
    public static AreaTweener Instance { get; private set; } = null;
    private CinemachineConfiner2D cinemachineConfiner2D;

    private void Awake()
    {
        Instance = this;
        // cinemachineConfiner2D = GameObject.Find("CM Camera").GetComponent<CinemachineConfiner2D>();
    }

    public void ChangeArea(Transform areaDoor)
    {
        AreaDoor targetArea = areaDoor.GetComponent<AreaDoor>();
        targetArea.originalAreaObject.SetActive(false);
        targetArea.targetAreaObject.SetActive(true);
        // cinemachineConfiner2D.m_BoundingShape2D = targetArea.Equals(AreaType.Normal) ? null : targetArea.compositeCollider2d;
    }
}
